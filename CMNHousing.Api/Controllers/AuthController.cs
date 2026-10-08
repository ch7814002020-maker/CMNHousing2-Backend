using CMNHousing.Api.Data;
using CMNHousing.Api.DTOs;
using CMNHousing.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMNHousing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // =========================================================
        // REGISTER
        // POST: api/Auth/register
        // Public - creates Customer account
        // =========================================================

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "Email already registered."
                });
            }

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone?.Trim(),

                // Password is stored as BCrypt hash
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    dto.Password),

                // Public registration always creates Customer
                Role = "Customer",

                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful.",
                userId = user.Id,
                name = user.FullName,
                email = user.Email,
                role = user.Role
            });
        }

        // =========================================================
        // LOGIN
        // POST: api/Auth/login
        // Public - returns JWT token
        // =========================================================

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new
                {
                    message = "User account is inactive."
                });
            }

            var passwordValid = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

            if (!passwordValid)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful.",
                token = token,

                user = new
                {
                    id = user.Id,
                    fullName = user.FullName,
                    email = user.Email,
                    phone = user.Phone,
                    role = user.Role
                }
            });
        }

        // =========================================================
        // CREATE ADMIN
        // POST: api/Auth/create-admin
        // ADMIN ONLY
        // =========================================================

        [Authorize(Roles = "Admin")]
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin(RegisterDto dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "Email already registered."
                });
            }

            var admin = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone?.Trim(),

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    dto.Password),

                Role = "Admin",

                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(admin);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Admin created successfully.",
                userId = admin.Id,
                name = admin.FullName,
                email = admin.Email,
                role = admin.Role
            });
        }

        // =========================================================
        // GENERATE JWT TOKEN
        // =========================================================

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT Key is missing.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT Issuer is missing.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT Audience is missing.");

            var expiresInMinutes = int.Parse(
                _configuration["Jwt:ExpiresInMinutes"] ?? "60");

            // =====================================================
            // JWT CLAIMS
            // =====================================================

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.FullName),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            // =====================================================
            // JWT KEY
            // =====================================================

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            // =====================================================
            // CREATE TOKEN
            // =====================================================

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expiresInMinutes),
                signingCredentials: credentials);

            // =====================================================
            // RETURN TOKEN STRING
            // =====================================================

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}

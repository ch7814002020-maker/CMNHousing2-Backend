using CMNHousing.Api.Data;
using CMNHousing.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMNHousing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnquiriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnquiriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET: api/Enquiries
        // Admin only
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetEnquiries()
        {
            var enquiries = await _context.Enquiries
                .Include(e => e.Property)
                .Include(e => e.User)
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return Ok(enquiries);
        }

        // =========================================================
        // GET: api/Enquiries/1
        // Admin only
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnquiry(int id)
        {
            var enquiry = await _context.Enquiries
                .Include(e => e.Property)
                .Include(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enquiry == null)
            {
                return NotFound(new
                {
                    message = "Enquiry not found"
                });
            }

            return Ok(enquiry);
        }

        // =========================================================
        // POST: api/Enquiries
        // Authenticated users can create enquiry
        // UserId comes automatically from JWT
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateEnquiry(
            [FromBody] Enquiry enquiry)
        {
            if (string.IsNullOrWhiteSpace(enquiry.Name))
            {
                return BadRequest(new
                {
                    message = "Name is required"
                });
            }

            if (string.IsNullOrWhiteSpace(enquiry.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required"
                });
            }

            if (string.IsNullOrWhiteSpace(enquiry.Message))
            {
                return BadRequest(new
                {
                    message = "Message is required"
                });
            }

            // Validate PropertyId if provided
            if (enquiry.PropertyId.HasValue)
            {
                var propertyExists = await _context.Properties
                    .AnyAsync(p => p.Id == enquiry.PropertyId.Value);

                if (!propertyExists)
                {
                    return BadRequest(new
                    {
                        message = "Invalid PropertyId"
                    });
                }
            }

            // =====================================================
            // Get UserId from JWT
            // Never trust UserId from request body
            // =====================================================
            var userIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            // Create enquiry
            var newEnquiry = new Enquiry
            {
                PropertyId = enquiry.PropertyId,

                // Secure UserId from JWT
                UserId = userId,

                Name = enquiry.Name.Trim(),
                Email = enquiry.Email.Trim(),
                Phone = enquiry.Phone?.Trim(),
                Message = enquiry.Message.Trim(),

                // Always start as New
                Status = "New",

                CreatedAt = DateTime.UtcNow
            };

            _context.Enquiries.Add(newEnquiry);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnquiry),
                new { id = newEnquiry.Id },
                newEnquiry);
        }

        // =========================================================
        // PUT: api/Enquiries/1
        // Admin only
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnquiry(
            int id,
            [FromBody] Enquiry enquiry)
        {
            var existingEnquiry = await _context.Enquiries
                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEnquiry == null)
            {
                return NotFound(new
                {
                    message = "Enquiry not found"
                });
            }

            if (string.IsNullOrWhiteSpace(enquiry.Name) ||
                string.IsNullOrWhiteSpace(enquiry.Email) ||
                string.IsNullOrWhiteSpace(enquiry.Message))
            {
                return BadRequest(new
                {
                    message = "Name, Email and Message are required"
                });
            }

            // Validate PropertyId
            if (enquiry.PropertyId.HasValue)
            {
                var propertyExists = await _context.Properties
                    .AnyAsync(p => p.Id == enquiry.PropertyId.Value);

                if (!propertyExists)
                {
                    return BadRequest(new
                    {
                        message = "Invalid PropertyId"
                    });
                }
            }

            existingEnquiry.PropertyId = enquiry.PropertyId;
            existingEnquiry.Name = enquiry.Name.Trim();
            existingEnquiry.Email = enquiry.Email.Trim();
            existingEnquiry.Phone = enquiry.Phone?.Trim();
            existingEnquiry.Message = enquiry.Message.Trim();

            // UserId is intentionally not changed
            if (!string.IsNullOrWhiteSpace(enquiry.Status))
            {
                existingEnquiry.Status = enquiry.Status.Trim();
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================================================
        // PATCH: api/Enquiries/1/status?status=Contacted
        // Admin only
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromQuery] string status)
        {
            var enquiry = await _context.Enquiries
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enquiry == null)
            {
                return NotFound(new
                {
                    message = "Enquiry not found"
                });
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new
                {
                    message = "Status is required"
                });
            }

            enquiry.Status = status.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Enquiry status updated successfully",
                status = enquiry.Status
            });
        }

        // =========================================================
        // DELETE: api/Enquiries/1
        // Admin only
        // =========================================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnquiry(int id)
        {
            var enquiry = await _context.Enquiries
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enquiry == null)
            {
                return NotFound(new
                {
                    message = "Enquiry not found"
                });
            }

            _context.Enquiries.Remove(enquiry);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
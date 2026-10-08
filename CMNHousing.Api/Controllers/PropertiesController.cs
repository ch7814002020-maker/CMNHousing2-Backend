using CMNHousing.Api.Data;
using CMNHousing.Api.DTOs;
using CMNHousing.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMNHousing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Properties
        // Customer + Admin
        [HttpGet]
        public async Task<IActionResult> GetProperties()
        {
            var properties = await _context.Properties
                .Include(p => p.Category)
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Include(p => p.Amenities)
                .ToListAsync();

            return Ok(properties);
        }

        // GET: api/Properties/1
        // Customer + Admin
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProperty(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Category)
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Include(p => p.Amenities)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound(new
                {
                    message = "Property not found"
                });
            }

            return Ok(property);
        }

        // POST: api/Properties
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProperty(
            [FromBody] CreatePropertyDto dto)
        {
            var categoryExists = await _context.PropertyCategories
                .AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    message = "Invalid CategoryId. Please create a category first."
                });
            }

            var locationExists = await _context.Locations
                .AnyAsync(l => l.Id == dto.LocationId);

            if (!locationExists)
            {
                return BadRequest(new
                {
                    message = "Invalid LocationId. Please create a location first."
                });
            }

            var property = new Property
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                PropertyType = dto.PropertyType,
                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms,
                Area = dto.Area,
                AreaUnit = dto.AreaUnit,
                Address = dto.Address,
                CategoryId = dto.CategoryId,
                LocationId = dto.LocationId,
                IsFeatured = dto.IsFeatured,
                IsAvailable = dto.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            _context.Properties.Add(property);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProperty),
                new { id = property.Id },
                property);
        }

        // PUT: api/Properties/1
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(
            int id,
            [FromBody] UpdatePropertyDto dto)
        {
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound(new
                {
                    message = "Property not found"
                });
            }

            var categoryExists = await _context.PropertyCategories
                .AnyAsync(c => c.Id == dto.CategoryId);

            var locationExists = await _context.Locations
                .AnyAsync(l => l.Id == dto.LocationId);

            if (!categoryExists || !locationExists)
            {
                return BadRequest(new
                {
                    message = "Invalid CategoryId or LocationId"
                });
            }

            property.Title = dto.Title;
            property.Description = dto.Description;
            property.Price = dto.Price;
            property.PropertyType = dto.PropertyType;
            property.Bedrooms = dto.Bedrooms;
            property.Bathrooms = dto.Bathrooms;
            property.Area = dto.Area;
            property.AreaUnit = dto.AreaUnit;
            property.Address = dto.Address;
            property.CategoryId = dto.CategoryId;
            property.LocationId = dto.LocationId;
            property.IsFeatured = dto.IsFeatured;
            property.IsAvailable = dto.IsAvailable;
            property.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Properties/1
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound(new
                {
                    message = "Property not found"
                });
            }

            _context.Properties.Remove(property);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
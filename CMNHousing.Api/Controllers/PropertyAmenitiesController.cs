using CMNHousing.Api.Data;
using CMNHousing.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CMNHousing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyAmenitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropertyAmenitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PropertyAmenities
        [HttpGet]
        public async Task<IActionResult> GetAmenities()
        {
            var amenities = await _context.PropertyAmenities
                .AsNoTracking()
                .ToListAsync();

            return Ok(amenities);
        }

        // GET: api/PropertyAmenities/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAmenity(int id)
        {
            var amenity = await _context.PropertyAmenities
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (amenity == null)
            {
                return NotFound(new
                {
                    message = "Amenity not found"
                });
            }

            return Ok(amenity);
        }

        // GET: api/PropertyAmenities/property/1
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetAmenitiesByProperty(int propertyId)
        {
            var propertyExists = await _context.Properties
                .AnyAsync(p => p.Id == propertyId);

            if (!propertyExists)
            {
                return NotFound(new
                {
                    message = "Property not found"
                });
            }

            var amenities = await _context.PropertyAmenities
                .AsNoTracking()
                .Where(a => a.PropertyId == propertyId)
                .ToListAsync();

            return Ok(amenities);
        }

        // POST: api/PropertyAmenities
        [HttpPost]
        public async Task<IActionResult> CreateAmenity(
            [FromBody] PropertyAmenity amenity)
        {
            if (string.IsNullOrWhiteSpace(amenity.AmenityName))
            {
                return BadRequest(new
                {
                    message = "AmenityName is required"
                });
            }

            var propertyExists = await _context.Properties
                .AnyAsync(p => p.Id == amenity.PropertyId);

            if (!propertyExists)
            {
                return BadRequest(new
                {
                    message = "Invalid PropertyId"
                });
            }

            var newAmenity = new PropertyAmenity
            {
                PropertyId = amenity.PropertyId,
                AmenityName = amenity.AmenityName
            };

            _context.PropertyAmenities.Add(newAmenity);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAmenity),
                new { id = newAmenity.Id },
                newAmenity);
        }

        // PUT: api/PropertyAmenities/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAmenity(
            int id,
            [FromBody] PropertyAmenity amenity)
        {
            var existingAmenity = await _context.PropertyAmenities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingAmenity == null)
            {
                return NotFound(new
                {
                    message = "Amenity not found"
                });
            }

            if (string.IsNullOrWhiteSpace(amenity.AmenityName))
            {
                return BadRequest(new
                {
                    message = "AmenityName is required"
                });
            }

            var propertyExists = await _context.Properties
                .AnyAsync(p => p.Id == amenity.PropertyId);

            if (!propertyExists)
            {
                return BadRequest(new
                {
                    message = "Invalid PropertyId"
                });
            }

            existingAmenity.PropertyId = amenity.PropertyId;
            existingAmenity.AmenityName = amenity.AmenityName;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PropertyAmenities/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAmenity(int id)
        {
            var amenity = await _context.PropertyAmenities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (amenity == null)
            {
                return NotFound(new
                {
                    message = "Amenity not found"
                });
            }

            _context.PropertyAmenities.Remove(amenity);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
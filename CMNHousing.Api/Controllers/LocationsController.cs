
using CMNHousing.Api.Data;
using CMNHousing.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMNHousing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    
    public class LocationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Locations
        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _context.Locations.ToListAsync();
            return Ok(locations);
        }

        // GET: api/Locations/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocation(int id)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                return NotFound(new { message = "Location not found" });
            }

            return Ok(location);
        }

        // POST: api/Locations
        [HttpPost]
        public async Task<IActionResult> CreateLocation(
            [FromBody] Location location)
        {
            if (string.IsNullOrWhiteSpace(location.City) ||
                string.IsNullOrWhiteSpace(location.Area) ||
                string.IsNullOrWhiteSpace(location.State))
            {
                return BadRequest(new
                {
                    message = "City, Area and State are required"
                });
            }

            var newLocation = new Location
            {
                City = location.City,
                Area = location.Area,
                State = location.State,
                Pincode = location.Pincode,
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };

            _context.Locations.Add(newLocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLocation),
                new { id = newLocation.Id },
                newLocation);
        }

        // PUT: api/Locations/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(
            int id,
            [FromBody] Location location)
        {
            var existingLocation = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == id);

            if (existingLocation == null)
            {
                return NotFound(new { message = "Location not found" });
            }

            if (string.IsNullOrWhiteSpace(location.City) ||
                string.IsNullOrWhiteSpace(location.Area) ||
                string.IsNullOrWhiteSpace(location.State))
            {
                return BadRequest(new
                {
                    message = "City, Area and State are required"
                });
            }

            existingLocation.City = location.City;
            existingLocation.Area = location.Area;
            existingLocation.State = location.State;
            existingLocation.Pincode = location.Pincode;
            existingLocation.Latitude = location.Latitude;
            existingLocation.Longitude = location.Longitude;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Locations/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
            {
                return NotFound(new { message = "Location not found" });
            }

            var hasProperties = await _context.Properties
                .AnyAsync(p => p.LocationId == id);

            if (hasProperties)
            {
                return BadRequest(new
                {
                    message = "Cannot delete location because properties are linked to it"
                });
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
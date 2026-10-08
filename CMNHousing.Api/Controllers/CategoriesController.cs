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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.PropertyCategories
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.PropertyCategories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            return Ok(category);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] PropertyCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest(new
                {
                    message = "Category name is required"
                });
            }

            var exists = await _context.PropertyCategories
                .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());

            if (exists)
            {
                return BadRequest(new
                {
                    message = "Category already exists"
                });
            }

            var newCategory = new PropertyCategory
            {
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.PropertyCategories.Add(newCategory);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = newCategory.Id },
                newCategory);
        }

        // PUT: api/Categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] PropertyCategory category)
        {
            var existingCategory = await _context.PropertyCategories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest(new
                {
                    message = "Category name is required"
                });
            }

            var duplicate = await _context.PropertyCategories
                .AnyAsync(c =>
                    c.Id != id &&
                    c.Name.ToLower() == category.Name.ToLower());

            if (duplicate)
            {
                return BadRequest(new
                {
                    message = "Another category with this name already exists"
                });
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.IsActive = category.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.PropertyCategories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found"
                });
            }

            var hasProperties = await _context.Properties
                .AnyAsync(p => p.CategoryId == id);

            if (hasProperties)
            {
                return BadRequest(new
                {
                    message = "Cannot delete category because properties are linked to it"
                });
            }

            _context.PropertyCategories.Remove(category);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
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
    public class PropertyImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PropertyImagesController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/PropertyImages
        // Customer + Admin
        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var images = await _context.PropertyImages
                .ToListAsync();

            return Ok(images);
        }

        // GET: api/PropertyImages/1
        // Customer + Admin
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _context.PropertyImages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (image == null)
            {
                return NotFound(new
                {
                    message = "Image not found"
                });
            }

            return Ok(image);
        }

        // GET: api/PropertyImages/property/1
        // Customer + Admin
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetPropertyImages(int propertyId)
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

            var images = await _context.PropertyImages
                .Where(x => x.PropertyId == propertyId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            return Ok(images);
        }

        // POST: api/PropertyImages/upload
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage(
            [FromForm] UploadPropertyImageDto dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)
            {
                return BadRequest(new
                {
                    message = "Please select an image."
                });
            }

            var propertyExists = await _context.Properties
                .AnyAsync(p => p.Id == dto.PropertyId);

            if (!propertyExists)
            {
                return BadRequest(new
                {
                    message = "Property not found."
                });
            }

            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension = Path.GetExtension(dto.Image.FileName)
                .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new
                {
                    message = "Only JPG, JPEG, PNG and WEBP images are allowed."
                });
            }

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName =
                $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(
                uploadsFolder,
                uniqueFileName);

            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            var imageUrl =
                $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";

            var propertyImage = new PropertyImage
            {
                PropertyId = dto.PropertyId,
                ImageUrl = imageUrl,
                AltText = dto.AltText,
                IsPrimary = dto.IsPrimary,
                DisplayOrder = dto.DisplayOrder,
                CreatedAt = DateTime.UtcNow
            };

            _context.PropertyImages.Add(propertyImage);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Image uploaded successfully.",
                imageId = propertyImage.Id,
                propertyId = propertyImage.PropertyId,
                imageUrl = propertyImage.ImageUrl,
                altText = propertyImage.AltText,
                isPrimary = propertyImage.IsPrimary,
                displayOrder = propertyImage.DisplayOrder
            });
        }

        // PUT: api/PropertyImages/1
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(
            int id,
            [FromBody] PropertyImage image)
        {
            var existingImage = await _context.PropertyImages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingImage == null)
            {
                return NotFound(new
                {
                    message = "Image not found"
                });
            }

            var propertyExists = await _context.Properties
                .AnyAsync(p => p.Id == image.PropertyId);

            if (!propertyExists)
            {
                return BadRequest(new
                {
                    message = "Property not found."
                });
            }

            existingImage.PropertyId = image.PropertyId;
            existingImage.ImageUrl = image.ImageUrl;
            existingImage.AltText = image.AltText;
            existingImage.IsPrimary = image.IsPrimary;
            existingImage.DisplayOrder = image.DisplayOrder;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PropertyImages/1
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.PropertyImages
                .FirstOrDefaultAsync(x => x.Id == id);

            if (image == null)
            {
                return NotFound(new
                {
                    message = "Image not found"
                });
            }

            // Delete physical file
            if (!string.IsNullOrEmpty(image.ImageUrl))
            {
                var fileName = Path.GetFileName(
                    new Uri(image.ImageUrl).AbsolutePath);

                var filePath = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.PropertyImages.Remove(image);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
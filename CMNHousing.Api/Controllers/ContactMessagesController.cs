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
    public class ContactMessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ContactMessages
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _context.ContactMessages
                .AsNoTracking()
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }

        // GET: api/ContactMessages/1
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessage(int id)
        {
            var message = await _context.ContactMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound(new
                {
                    message = "Contact message not found"
                });
            }

            return Ok(message);
        }

        // POST: api/ContactMessages
        // Any authenticated user
        [HttpPost]
        public async Task<IActionResult> CreateMessage(
            [FromBody] ContactMessage contactMessage)
        {
            if (string.IsNullOrWhiteSpace(contactMessage.Name))
            {
                return BadRequest(new
                {
                    message = "Name is required"
                });
            }

            if (string.IsNullOrWhiteSpace(contactMessage.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required"
                });
            }

            if (string.IsNullOrWhiteSpace(contactMessage.Message))
            {
                return BadRequest(new
                {
                    message = "Message is required"
                });
            }

            var newMessage = new ContactMessage
            {
                Name = contactMessage.Name,
                Email = contactMessage.Email,
                Phone = contactMessage.Phone,
                Subject = contactMessage.Subject,
                Message = contactMessage.Message,
                Status = "New",
                CreatedAt = DateTime.UtcNow
            };

            _context.ContactMessages.Add(newMessage);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMessage),
                new { id = newMessage.Id },
                newMessage);
        }

        // PUT: api/ContactMessages/1
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMessage(
            int id,
            [FromBody] ContactMessage contactMessage)
        {
            var existingMessage = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existingMessage == null)
            {
                return NotFound(new
                {
                    message = "Contact message not found"
                });
            }

            if (string.IsNullOrWhiteSpace(contactMessage.Name) ||
                string.IsNullOrWhiteSpace(contactMessage.Email) ||
                string.IsNullOrWhiteSpace(contactMessage.Message))
            {
                return BadRequest(new
                {
                    message = "Name, Email and Message are required"
                });
            }

            existingMessage.Name = contactMessage.Name;
            existingMessage.Email = contactMessage.Email;
            existingMessage.Phone = contactMessage.Phone;
            existingMessage.Subject = contactMessage.Subject;
            existingMessage.Message = contactMessage.Message;
            existingMessage.Status = contactMessage.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/ContactMessages/1/status
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromQuery] string status)
        {
            var contactMessage = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contactMessage == null)
            {
                return NotFound(new
                {
                    message = "Contact message not found"
                });
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new
                {
                    message = "Status is required"
                });
            }

            contactMessage.Status = status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Contact message status updated successfully",
                status = contactMessage.Status
            });
        }

        // DELETE: api/ContactMessages/1
        // Admin only
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var contactMessage = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contactMessage == null)
            {
                return NotFound(new
                {
                    message = "Contact message not found"
                });
            }

            _context.ContactMessages.Remove(contactMessage);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

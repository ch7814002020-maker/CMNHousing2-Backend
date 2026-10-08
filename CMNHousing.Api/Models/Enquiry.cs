using System.ComponentModel.DataAnnotations;

namespace CMNHousing.Api.Models
{
    public class Enquiry
    {
        public int Id { get; set; }

        public int? PropertyId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Status { get; set; } = "New";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Enquiry → Property
        public Property? Property { get; set; }

        // Enquiry → User
        public User? User { get; set; }
    }
}
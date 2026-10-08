using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CMNHousing.Api.DTOs
{
    public class UploadPropertyImageDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;

        [MaxLength(200)]
        public string? AltText { get; set; }

        public bool IsPrimary { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;
    }
}
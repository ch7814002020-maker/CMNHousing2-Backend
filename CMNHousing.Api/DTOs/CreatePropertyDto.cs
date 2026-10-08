using System.ComponentModel.DataAnnotations;

namespace CMNHousing.Api.DTOs
{
    public class CreatePropertyDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string PropertyType { get; set; } = string.Empty;

        [Range(0, 100)]
        public int Bedrooms { get; set; }

        [Range(0, 100)]
        public int Bathrooms { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Area { get; set; }

        public string AreaUnit { get; set; } = "SqFt";

        public string? Address { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int LocationId { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
namespace CMNHousing.Api.Models
{
    public class Property
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string PropertyType { get; set; } = string.Empty;

        public int Bedrooms { get; set; }

        public int Bathrooms { get; set; }

        public decimal Area { get; set; }

        public string AreaUnit { get; set; } = "SqFt";

        public string? Address { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }

        public int LocationId { get; set; }

        // Navigation Properties
        public PropertyCategory Category { get; set; } = null!;

        public Location Location { get; set; } = null!;

        public bool IsFeatured { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<PropertyImage> Images { get; set; }
    = new List<PropertyImage>();

        public ICollection<PropertyAmenity> Amenities { get; set; }
            = new List<PropertyAmenity>();
    }

}
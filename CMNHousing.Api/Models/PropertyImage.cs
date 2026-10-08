namespace CMNHousing.Api.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string? AltText { get; set; }

        public bool IsPrimary { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Property Property { get; set; } = null!;
    }
}
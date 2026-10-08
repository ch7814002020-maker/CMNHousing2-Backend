using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CMNHousing.Api.Models
{
    public class PropertyCategory
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Property> Properties { get; set; }
            = new List<Property>();
    }
}
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CMNHousing.Api.Models
{
    public class Location
    {
        public int Id { get; set; }

        public string City { get; set; } = string.Empty;

        public string Area { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string? Pincode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public ICollection<Property> Properties { get; set; }
            = new List<Property>();
    }
}
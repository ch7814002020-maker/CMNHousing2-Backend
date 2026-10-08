namespace CMNHousing.Api.Models
{
    public class PropertyAmenity
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public string AmenityName { get; set; } = string.Empty;

        public Property? Property { get; set; }
    }
}
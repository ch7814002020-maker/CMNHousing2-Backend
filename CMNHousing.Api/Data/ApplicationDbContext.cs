using CMNHousing.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CMNHousing.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<PropertyCategory> PropertyCategories { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Property> Properties { get; set; }

        public DbSet<PropertyImage> PropertyImages { get; set; }

        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }

        public DbSet<Enquiry> Enquiries { get; set; }

        public DbSet<ContactMessage> ContactMessages { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // ============================================
            // Category → Properties
            // ============================================

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Properties)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================
            // Location → Properties
            // ============================================

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Location)
                .WithMany(l => l.Properties)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);


            // ============================================
            // Property → Images
            // ============================================

            modelBuilder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================================
            // Property → Amenities
            // ============================================

            modelBuilder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Property)
                .WithMany(p => p.Amenities)
                .HasForeignKey(pa => pa.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);


            // ============================================
            // Enquiry → Property
            // ============================================

            modelBuilder.Entity<Enquiry>()
                .HasOne(e => e.Property)
                .WithMany()
                .HasForeignKey(e => e.PropertyId)
                .OnDelete(DeleteBehavior.SetNull);


            // ============================================
            // Enquiry → User
            // ============================================

            modelBuilder.Entity<Enquiry>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);


            // ============================================
            // Property Price Precision
            // ============================================

            modelBuilder.Entity<Property>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);


            // ============================================
            // Property Area Precision
            // ============================================

            modelBuilder.Entity<Property>()
                .Property(p => p.Area)
                .HasPrecision(12, 2);


            // ============================================
            // Location Latitude Precision
            // ============================================

            modelBuilder.Entity<Location>()
                .Property(l => l.Latitude)
                .HasPrecision(10, 7);


            // ============================================
            // Location Longitude Precision
            // ============================================

            modelBuilder.Entity<Location>()
                .Property(l => l.Longitude)
                .HasPrecision(10, 7);
        }
    }
}

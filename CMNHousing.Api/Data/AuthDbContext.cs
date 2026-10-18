using CMNHousing.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace CMNHousing.AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
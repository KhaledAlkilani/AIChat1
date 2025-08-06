using Microsoft.EntityFrameworkCore;

namespace AIChat1
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // Define DbSets for your entities here
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}

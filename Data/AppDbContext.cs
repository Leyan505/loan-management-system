using Microsoft.EntityFrameworkCore;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Data
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string frunaom app settings
            options.UseNpgsql(Configuration.GetConnectionString("PrestamosDB"));
        }

        public DbSet<Users> Users { get; set; }
    }
}

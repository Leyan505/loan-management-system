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
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<SupervisorHasAgent> AgentSupervisor { get; set; }
        public DbSet<Credit> Credit {  get; set; }
        public DbSet<AgentHasClient> AgentHasClient { get; set; }
        public DbSet<Summary> Summary { get; set; }
        public DbSet<CloseDay> CloseDay { get; set; }
        public DbSet<Bills> Bills { get; set; }
        public DbSet<ListBill> ListBills { get; set; }
        public DbSet<Countries> Countries { get; set; }
    }
}

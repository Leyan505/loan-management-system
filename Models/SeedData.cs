using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PrestamosCreciendo.Data;
using System.Configuration;

namespace PrestamosCreciendo.Models
{
    public static class SeedData
    {
        public static void Initialize(IConfiguration configuration)
        {
            using (var context = new AppDbContext(configuration))
            {
                // Look for any movies.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }
                context.Users.AddRange(
                    new Users
                    {
                        Name = "admin",
                        Email = "admin@admin.com",
                        Password = "1234",
                        Level = "admin"
                    },
                    new Users
                    {
                        Name = "supervisor",
                        Email = "supervisor@supervisor.com",
                        Password = "1234",
                        Level = "supervisor"
                    },
                    new Users
                    {
                        Name = "agente",
                        Email = "agente@agente.com",
                        Password = "1234",
                        Level = "agente"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                // Look for any users.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }
                context.Users.AddRange(
                    new Users
                    {
                        Name = "admin",
                        Email = "admin@admin.com",
                        Password = "12345678",
                        Level = "admin"
                    },
                    new Users
                    {
                        Name = "supervisor",
                        Email = "supervisor@supervisor.com",
                        Password = "12345678",
                        Level = "supervisor"
                    },
                    new Users
                    {
                        Name = "agente",
                        Email = "agente@agente.com",
                        Password = "12345678",
                        Level = "agente",
                        Country = "Nicaragua",
                        City = "Managua"
                    }
                );
                // Look for any wallets.
                if (context.Wallets.Any())
                {
                    return;   // DB has been seeded
                }
                context.Wallets.AddRange(
                    new Wallet
                    {
                        Name = "caja principal",
                        Country = 1,
                        City = "Managua"
                    },
                    new Wallet
                    {
                        Name = "Caja secundaria",
                        Country = 1,
                        City = "Managua"
                    }
                );
                context.SaveChanges();

                // Look for any wallets.
                if (context.AgentSupervisor.Any())
                {
                    return;   // DB has been seeded
                }
                context.AgentSupervisor.AddRange(
                    new SupervisorHasAgent
                    {
                        IdAgent = 3,
                        IdSupervisor = 2,
                        IdWallet = 1,
                        Base = 10000
                    }
                );
                context.SaveChanges();

                if (context.ListBills.Any())
                {
                    return;   // DB has been seeded
                }
                context.ListBills.AddRange(
                    new ListBill
                    {
                        Name = "Combustible",
                    },
                    new ListBill
                    {
                        Name = "Comida",
                    },
                    new ListBill
                    {
                        Name = "Transporte",
                    }
                );
                context.SaveChanges();

                if (context.Countries.Any())
                {
                    return;   // DB has been seeded
                }
                context.Countries.AddRange(
                    new Countries
                    {
                        Name = "Nicaragua",
                    },
                    new Countries
                    {
                        Name = "USA",
                    }
                );
                context.SaveChanges();
            }
        }
    }
}

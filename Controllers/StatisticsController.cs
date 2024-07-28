using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Model;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            List<SupervisorHasAgentDTO> data = (from supag in _context.AgentSupervisor
                                               where supag.IdSupervisor == CurrentUser.Id
                                               join users in _context.Users on supag.IdAgent equals users.Id
                                               join wallet in _context.Wallets on supag.IdWallet equals wallet.Id
                                               select new SupervisorHasAgentDTO()
                                               {
                                                   user = users,
                                                   wallet_name = wallet.Name,
                                               }).ToList();

            return View(data);
        }

        public IActionResult Create(int id_agent)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            return View(id_agent);
        }

        public IActionResult Show(int id, DateTime date_start, DateTime date_end)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            var data_supervisor = (from supag in _context.AgentSupervisor
                        where supag.IdAgent == id
                        select supag).FirstOrDefault();
            Wallet? data_wallet = (from wallet in _context.Wallets
                    where wallet.Id == data_supervisor.IdWallet
                    select wallet).FirstOrDefault();
            
            float summary = (from sum in _context.Summary
                            where sum.Id_agent == id
                            && sum.Created_at.Date >= date_start.ToUniversalTime().Date
                            && sum.Created_at.Date <= date_end.ToUniversalTime().Date
                            select sum).Sum(x => x.Amount);

            float credit = (from cred in _context.Credit
                            where cred.Id_agent == id
                            && cred.Created_at.Date >= date_start.ToUniversalTime().Date
                            && cred.Created_at.Date <= date_end.ToUniversalTime().Date
                            select cred).Sum(x => x.Amount_neto);

            float bills = (from bill in _context.Bills
                            where bill.Id_agent == id
                            && bill.Created_at.Date >= date_start.ToUniversalTime().Date
                            && bill.Created_at.Date <= date_end.ToUniversalTime().Date
                            select bill).Sum(x => x.Amount);
            int days = date_end.DayOfYear - date_start.DayOfYear+1;

            StatisticsDTO data = new StatisticsDTO()
            {
                summary = summary,
                bills = bills,
                credit = credit,
                days = days,
                wallet = data_wallet,
                range = new string("Desde " + date_start.ToLocalTime().ToShortDateString()
                + " hasta " + date_end.ToLocalTime().ToShortDateString())
            };
            return View(data);
        }

    }
}

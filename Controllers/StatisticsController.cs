using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "SupervisorOnly")]
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
            ViewData["Name"] = CurrentUser.Name;

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
            ViewData["Name"] = CurrentUser.Name;

            return View(id_agent);
        }

        public IActionResult Show(int id, DateTime date_start, DateTime date_end)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;



            var data_supervisor = (from supag in _context.AgentSupervisor
                        where supag.IdAgent == id
                        select supag).FirstOrDefault();
            Wallet? data_wallet = (from wallet in _context.Wallets
                    where wallet.Id == data_supervisor.IdWallet
                    select wallet).FirstOrDefault();


            DateTime date_startSooner= date_start.AddDays(-1);
            DateTime date_endlater = date_end.AddDays(1);

            DateTime DtNow = DateOffset.DateNow(DateTime.UtcNow, CurrentUser.TimeOffset);


            var materializedSummary = _context.Summary.Where(x => x.Created_at.Date <= date_endlater && x.Created_at >= date_startSooner.Date).ToList();
            foreach (var Item in materializedSummary)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            float summary = (from sum in materializedSummary
                            where sum.Id_agent == id
                            && sum.Created_at.Date >= date_start.Date
                            && sum.Created_at.Date <= date_end.Date
                            select sum).Sum(x => x.Amount);


            var materializedCredit = _context.Credit.Where(x => x.Created_at.Date <= date_endlater && x.Created_at >= date_startSooner.Date).ToList();
            foreach (var Item in materializedCredit)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }


            float credit = (from cred in materializedCredit
                            where cred.Id_agent == id
                            && cred.Created_at.Date >= date_start.Date
                            && cred.Created_at.Date <= date_end.Date
                            select cred).Sum(x => x.Amount_neto);



            var materializedBills= _context.Bills.Where(x => x.Created_at.Date <= date_endlater && x.Created_at >= date_startSooner.Date).ToList();
            foreach (var Item in materializedBills)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            float bills = (from bill in materializedBills
                            where bill.Id_agent == id
                            && bill.Created_at.Date >= date_start.Date
                            && bill.Created_at.Date <= date_end.Date
                            select bill).Sum(x => x.Amount);


            int days = date_startSooner.DayOfYear - date_start.DayOfYear+1;

            StatisticsDTO data = new StatisticsDTO()
            {
                summary = summary,
                bills = bills,
                credit = credit,
                days = days,
                wallet = data_wallet,
                range = new string("Desde " + date_start.ToShortDateString()
                + " hasta " + date_end.ToShortDateString())
            };
            return View(data);
        }

    }
}

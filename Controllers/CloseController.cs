using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "SupervisorOnly")]
    public class CloseController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public CloseController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            List<CloseDTO> data = _context.AgentSupervisor.Where(x => x.IdSupervisor == CurrentUser.Id)
                                  .Join(_context.Users, agentSup => agentSup.IdAgent, user => user.Id,
                                  (agentSup, user) => new CloseDTO()
                                  {
                                      SupervisorHasAgent = agentSup,
                                      users = user
                                  }).ToList();
            DateTime DtNow = DateOffset.DateNow(DateTime.UtcNow, CurrentUser.TimeOffset);
            DateTime date_startGreater = DateTime.UtcNow.AddDays(1).Date;
            DateTime date_end = DateTime.UtcNow.AddDays(-1).Date;

            foreach (var datum in data)
            {
                datum.Show = true;
                datum.wallet_name = _context.Wallets.Where(x => x.Id == datum.SupervisorHasAgent.IdWallet)
                                    .FirstOrDefault().Name;

                var materializedSummary = _context.Summary.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();
                foreach (var Item in materializedSummary)
                {
                    Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
                }
                var summary = materializedSummary.Where(x => x.Created_at.Date == DtNow.Date
                && x.Id_agent == datum.SupervisorHasAgent.IdAgent).Any();

                if (summary) { datum.Show = true; }

                var materializedCredit = _context.Credit.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();
                foreach (var Item in materializedCredit)
                {
                    Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
                }
                var credit = materializedCredit.Where(x => x.Created_at.Date == DtNow.Date
                             && x.Id_agent == datum.SupervisorHasAgent.IdAgent).Any();

                if (credit) { datum.Show = true; }

                var materializedClose = _context.Credit.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();
                foreach (var Item in materializedClose)
                {
                    Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
                }
                var close_day = materializedClose.Where(x => x.Id_agent == datum.SupervisorHasAgent.IdAgent
                && x.Created_at.Date == DtNow.Date).Any();

                if (close_day)
                {
                    datum.Show = false;
                }

            }
            data.FirstOrDefault().dtnow = DtNow.ToShortDateString();
            return View(data);
        }

        public IActionResult Close(int id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            DateTime DtNow = DateOffset.DateNow(DateTime.UtcNow, CurrentUser.TimeOffset);
            DateTime date_startGreater = DateTime.UtcNow.AddDays(1).Date;
            DateTime date_end = DateTime.UtcNow.AddDays(-1).Date;

            float base_amount = _context.AgentSupervisor.Where(x => x.IdAgent == id).FirstOrDefault().Base;

            var materializedSummary = _context.Summary.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();
            foreach (var Item in materializedSummary)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            float today_amount = materializedSummary.Where(x => x.Created_at.Date == DtNow.Date && x.Id_agent == id)
                .Sum(x => x.Amount);


            var materializedCredit = _context.Credit.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();
            foreach (var Item in materializedCredit)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }

            float today_sell = materializedCredit.Where(x => x.Created_at.Date == DtNow.Date && x.Id_agent == id)
                .Sum(x => x.Amount_neto);

            var materializedBill = _context.Bills.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();
            foreach (var Item in materializedBill)
            {
                Item.Created_at = DateOffset.DateNow(Item.Created_at, CurrentUser.TimeOffset);
            }
            float bills = materializedBill.Where(x => x.Created_at.Date == DtNow.Date).Sum(x => x.Amount);

            float total = (float)(base_amount + today_amount) - (float)(today_sell + bills);

            float average = 1000;

            var data = new HistoryAgentDTO()
            {
                Base = base_amount,
                today_amount = today_amount,
                today_sell = today_sell,
                bills = bills,
                total = total,
                average = average,
                user = _context.Users.Find(id)
            };


            return View(data);
        }
        [HttpPost]
        public IActionResult Close(float total_today, float base_amount_total, int id)
        {
            CurrentUser = new LoggedUser(HttpContext);

            if (total_today == 0)
            {
                return View(new HistoryAgentDTO() { error = new ErrorViewModel() { description = "Total vacio" } });
            }
            if (base_amount_total == 0)
            {
                return View(new HistoryAgentDTO() { error = new ErrorViewModel() { description = "Base vacio" } });
            }

            _context.AgentSupervisor.Where(x => x.IdAgent == id && x.IdSupervisor == CurrentUser.Id).ExecuteUpdate(x => x.SetProperty(x => x.Base, total_today));

            CloseDay values = new CloseDay()
            {
                Id_agent = id,
                Id_supervisor = CurrentUser.Id,
                Created_at = DateTime.UtcNow,
                Total = total_today,
                Base_before = base_amount_total

            };

            _context.CloseDay.Add(values);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}

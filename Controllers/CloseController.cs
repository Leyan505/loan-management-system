using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
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
            ViewData["Level"] = CurrentUser.Level;

            List<CloseDTO> data = _context.AgentSupervisor.Where(x => x.IdSupervisor == CurrentUser.Id)
                                  .Join(_context.Users, agentSup => agentSup.IdAgent, user => user.Id,
                                  (agentSup, user) => new CloseDTO()
                                  {
                                      SupervisorHasAgent = agentSup,
                                      users = user
                                  }).ToList();

            foreach (var datum in data)
            {
                datum.Show = true;
                datum.wallet_name = _context.Wallets.Where(x => x.Id == datum.SupervisorHasAgent.IdWallet)
                                    .FirstOrDefault().Name;

                var summary = _context.Summary.Where(x => x.Created_at.Date == DateTime.UtcNow.Date
                && x.Id_agent == datum.SupervisorHasAgent.IdAgent).Any();

                if (summary) { datum.Show = true; }

                var credit = _context.Credit.Where(x => x.Created_at.Date == DateTime.UtcNow.Date
                             && x.Id_agent == datum.SupervisorHasAgent.IdAgent).Any();

                if (credit) { datum.Show = true; }

                var close_day = _context.CloseDay.Where(x => x.Id_agent == datum.SupervisorHasAgent.IdAgent
                && x.Created_at.Date == DateTime.UtcNow.Date).Any();

                if (close_day)
                {
                    datum.Show = false;
                }

            }

            return View(data);
        }

        public IActionResult Close(int id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            float base_amount = _context.AgentSupervisor.Where(x => x.IdAgent == id).FirstOrDefault().Base;
            float today_amount = _context.Summary.Where(x => x.Created_at.Date == DateTime.UtcNow.Date && x.Id_agent == id)
                .Sum(x => x.Amount);
            float today_sell = _context.Credit.Where(x => x.Created_at.Date == DateTime.UtcNow.Date && x.Id_agent == id)
                .Sum(x => x.Amount_neto);

            float bills = _context.Bills.Where(x => x.Created_at.Date == DateTime.UtcNow.Date).Sum(x => x.Amount);

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

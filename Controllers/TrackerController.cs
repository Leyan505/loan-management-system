using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "SupervisorOnly")]
    public class TrackerController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public TrackerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            List<SupervisorHasAgentDTO> data = (from asup in _context.AgentSupervisor
                                                where asup.IdSupervisor == CurrentUser.Id
                                                join users in _context.Users on asup.IdAgent equals users.Id
                                                join wallets in _context.Wallets on asup.IdWallet equals wallets.Id
                                                select new SupervisorHasAgentDTO()
                                                {
                                                    user = users,
                                                    wallet_name = wallets.Name,
                                                }).ToList();

            return View(data);
        }

        public IActionResult Create(int id_agent)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;


            return View(id_agent);
        }
        public IActionResult Summary(int id, DateTime date_start)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            DateTime date_startGreater = date_start.AddDays(1);
            DateTime date_end = date_start.AddDays(-1);

            var materializedSummary = _context.Summary.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();

            List<TrackerSummaryDTO> data_summary = (from summary in materializedSummary
                                                    where summary.Created_at.Date == date_start.Date
                                                    join credit in _context.Credit on summary.Id_credit equals credit.Id
                                                    where credit.Id_agent == id
                                                    join users in _context.Users on credit.Id_user equals users.Id
                                                    select new TrackerSummaryDTO()
                                                    {
                                                        user_name = users.Name,
                                                        users_lastName = users.LastName,
                                                        payment_number = credit.Payment_number,
                                                        amount_neto = credit.Amount_neto,
                                                        id_credit = credit.Id,
                                                        number_index = summary.Number_index,
                                                        amount = summary.Amount,
                                                        created_at = summary.Created_at,

                                                    }).ToList();

            var materializedCredit = _context.Credit.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();


            List<TrackerCreditDTO> data_credit = (from credit in materializedCredit
                                                  where credit.Created_at.Date == date_start.Date
                                                  && credit.Id_agent == id
                                                  join users in _context.Users on credit.Id_user equals users.Id
                                                  select new TrackerCreditDTO()
                                                  {
                                                      credit_id = credit.Id,
                                                      user_id = users.Id,
                                                      user_lastName = users.LastName,
                                                      user_name = users.Name,
                                                      user_province = users.Province,
                                                      credit_amountNeto = credit.Amount_neto,
                                                      credit_created_at = credit.Created_at,
                                                      credit_paymentNumber = credit.Payment_number,
                                                      credit_utility = credit.Utility,
                                                  }
                                                  ).ToList();

            var materializedBill = _context.Bills.Where(x => x.Created_at.Date <= date_startGreater && x.Created_at >= date_end.Date).ToList();

            List<Bills> data_bill = (from bills in materializedBill
                                     where bills.Created_at.Date == date_start.Date
                                    && bills.Id_agent == id
                                    select bills).ToList();

            TrackerDTO data = new TrackerDTO()
            {
                Summary = data_summary,
                Credit = data_credit,
                Bills = data_bill,
                total_summary = data_summary.Sum(x => x.amount),
                total_credit = data_credit.Sum(x => x.credit_amountNeto)
            };

            return View(data);
        }
    }
}

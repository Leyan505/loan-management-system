using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
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
            ViewData["Level"] = CurrentUser.Level;

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
            ViewData["Level"] = CurrentUser.Level;


            return View(id_agent);
        }
        public IActionResult Summary(int id, DateTime date_start)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            List<TrackerSummaryDTO> data_summary = (from summary in _context.Summary
                                                    where summary.Created_at.Date == date_start.ToUniversalTime().Date
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

            List<TrackerCreditDTO> data_credit = (from credit in _context.Credit
                                                  where credit.Created_at.Date == date_start.ToUniversalTime().Date
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

            List<Bills> data_bill = (from bills in _context.Bills
                                    where bills.Created_at.Date == date_start.ToUniversalTime().Date
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

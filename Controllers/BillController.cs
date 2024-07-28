using Humanizer;
using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;
using System.Linq.Expressions;

namespace PrestamosCreciendo.Controllers
{
    public class BillController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public BillController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(DateTime? date_start, DateTime? date_end, int category)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            List<ListBill> list_categories = (from list in _context.ListBills
                                             select list).ToList();

            Expression<Func<PrestamosCreciendo.Models.Bills, bool>> sql = x => x.Id_agent == CurrentUser.Id;

            if(date_start != null && date_end != null)
            {
                sql = x => x.Id_agent == CurrentUser.Id && x.Created_at.Date >= date_start.Value.ToUniversalTime().Date
                && x.Created_at.Date <= date_end.Value.ToUniversalTime().Date;
            }
            else
            {
                sql = x => x.Id_agent == CurrentUser.Id && x.Created_at.Date == DateTime.UtcNow.Date;
            }

            List<BillsDTO> data = _context.Bills.Where(sql)
                                  .Join(_context.Wallets, wallet => wallet.Id_wallet, bill => bill.Id,
                                  (bill1, wallet) => new { bill1, wallet })
                                  .Join(_context.ListBills, bill => bill.bill1.Type, list_bill => list_bill.Id,
                                  (bill2, list_bill) => new { bill2, list_bill })
                                  .Join(_context.Users, bill => bill.bill2.bill1.Id_agent, users => users.Id,
                                  (bill3, users) => new { bill3, users })
                                  .Select(result => new BillsDTO()
                                  {
                                      bill = result.bill3.bill2.bill1,
                                      wallet_name = result.bill3.bill2.wallet.Name,
                                      category_name = result.bill3.list_bill.Name,
                                      user_name = result.users.Name
                                  }).ToList();

            if(category != 0)
            {
                data = data.Where(x => x.bill.Type == category).ToList();
            }

            BillsDTO dataBills = new BillsDTO()
            {
                clients = data,
                total = data.Sum(x => x.bill.Amount),
                list_categories = list_categories,
            };

            return View(dataBills);
        }
        public IActionResult Create()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            List<ListBill> data = _context.ListBills.Select(x => x).ToList();

            return View(data);
        }
        [HttpPost]
        public IActionResult Create(float amount, string description, int type_bill)
        {
            CurrentUser = new LoggedUser(HttpContext);

            SupervisorHasAgent wallet = _context.AgentSupervisor.Where(x => x.IdAgent == CurrentUser.Id).FirstOrDefault();

            Bills bill = new Bills()
            {
                Description = description,
                Id_agent = CurrentUser.Id,
                Amount = amount,
                Created_at = DateTime.UtcNow,
                Type = type_bill,
                Id_wallet = wallet.IdWallet
            };
            _context.Bills.Add(bill);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

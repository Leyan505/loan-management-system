using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;
using System.Linq.Expressions;

namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "SupervisorOnly")]
    public class SupervisorBillsController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public SupervisorBillsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(DateTime? date_start, DateTime? date_end, int category)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;
            List<ListBill> list_categories = (from list in _context.ListBills
                                              select list).ToList();

            var ormQry = (from supag in _context.AgentSupervisor
                         where supag.IdSupervisor == CurrentUser.Id
                         join wallet in _context.Wallets on supag.IdWallet equals wallet.Id
                         join bills in _context.Bills on wallet.Id equals bills.Id_wallet
                         join listbill in _context.ListBills on bills.Type equals listbill.Id
                         join users in _context.Users on bills.Id_agent equals users.Id
                         select new BillsDTO()
                         {
                             bills = bills,
                             category_name = listbill.Name,
                             user_name = users.Name,
                             wallet_name = wallet.Name,
                         }).ToList();

            var Sum = (from agsup in _context.AgentSupervisor
                          where agsup.IdSupervisor == CurrentUser.Id
                          join wallet in _context.Wallets on agsup.IdWallet equals wallet.Id
                          join bill in _context.Bills on wallet.Id equals bill.Id_wallet
                          select new { wallet, bill, agsup }).ToList();

            foreach (var ItemQry in ormQry)
            {
                ItemQry.bills.Created_at = DateOffset.DateNow(ItemQry.bills.Created_at, CurrentUser.TimeOffset);
            }

            if (date_start != null)
            {
                ormQry = ormQry.Where(x => x.bills.Created_at.Date >= date_start.Value.Date).ToList();
                Sum = Sum.Where(x => x.bill.Created_at.Date >= date_start.Value.Date).ToList();
            }
            if (date_end != null)
            {
                ormQry = ormQry.Where(x => x.bills.Created_at.Date <= date_end.Value.Date).ToList();
                Sum = Sum.Where(x => x.bill.Created_at.Date <= date_end.Value.Date).ToList();
            }

            if(category != 0)
            {
                ormQry = ormQry.Where(x=> x.bills.Type == category).ToList();
            }

            float sum = Sum.Sum(x => x.bill.Amount);

            BillsDTO data = new BillsDTO()
            {
                clients = ormQry,
                total = sum,
                list_categories = list_categories,
            };


            return View(data);
        }

        /*public IActionResult Edit()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            return View();
        }*/

        public IActionResult Create(String? Error)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            List<Wallet> data_wallet = (from supag in _context.AgentSupervisor
                                         where supag.IdSupervisor == CurrentUser.Id
                                         join wallet in _context.Wallets on supag.IdWallet equals wallet.Id
                                         select wallet).ToList();
            List<ListBill> data_bills = _context.ListBills.Select(x => x).ToList();

            List<Users> agents = (from supag in _context.AgentSupervisor
                                 where supag.IdSupervisor == CurrentUser.Id
                                 join users in _context.Users on supag.IdAgent equals users.Id
                                 select users).ToList();
            SupervisorBillsCreateDTO data = new SupervisorBillsCreateDTO()
            {
                bills = data_bills,
                wallets = data_wallet,
                agents = agents
            };

            if (Error != null) { data.error = new ErrorViewModel() { description = Error }; }

            return View(data);
        }
        [HttpPost]
        public IActionResult Create(int id_wallet, float amount, int bill, string description, int id_agent)
        {
            CurrentUser = new LoggedUser(HttpContext);
            if (!_context.AgentSupervisor.Where(x=> x.IdWallet == id_wallet && x.IdAgent == id_agent).Any())
            {
                return RedirectToAction("Create", new{Error = "Agente no corresponde en esa cartera" });
            }
            Bills value = new Bills()
            {
                Created_at = DateTime.UtcNow,
                Description = description,
                Id_wallet = id_wallet,
                Type = bill,
                Amount = amount,
                Id_agent = id_agent
            };

            _context.Bills.Add(value);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}

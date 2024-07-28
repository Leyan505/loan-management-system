using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
    public class CashController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public CashController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            List<SupervisorHasAgentDTO> data = (from supag in _context.AgentSupervisor
                                                where supag.IdSupervisor == CurrentUser.Id
                                                join wallets in _context.Wallets on supag.IdWallet equals wallets.Id
                                                select new SupervisorHasAgentDTO()
                                                {
                                                    wallet_name = wallets.Name,
                                                    base_total = supag.Base,
                                                    wallet_createdAt = wallets.Created_at

                                                }).ToList();

            float sum = (from supag in _context.AgentSupervisor 
                        join wallets in _context.Wallets on supag.IdWallet equals wallets.Id
                        select new {supag, wallets}).Sum(x => x.supag.Base);

            List<CloseDay> report = (from closeday in _context.CloseDay
                               where closeday.Id_supervisor == CurrentUser.Id
                               orderby closeday.Id descending
                               select closeday).ToList();


            return View(new CashDTO()
            {
                report = report,
                clients = data,
                sum = sum,
            });
        }
    }
}

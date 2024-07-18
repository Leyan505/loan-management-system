using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public SupervisorController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;
            return View();
        }

        public IActionResult AsignarBase()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;
            int? supervisorId = CurrentUser.Id;
            var query = (from agent_supervisor in _context.AgentSupervisor
                        join users in _context.Users on agent_supervisor.IdAgent equals users.Id
                        join wallet in _context.Wallets on agent_supervisor.IdWallet equals wallet.Id
                        select new AgentHasSupervisorDTO 
                        { 
                            AgentName = users.Name, 
                            WalletName = wallet.Name, 
                            City = users.City, 
                            Base = agent_supervisor.Base, 
                            Id = agent_supervisor.Id 
                        }).ToList();

            return View(query);
        }
        [HttpGet]
        public IActionResult UpdateBase(int Id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            var query = (from agent_supervisor in _context.AgentSupervisor
                         where agent_supervisor.Id == Id
                         select agent_supervisor).FirstOrDefault();

            return View(query);
        }
        [HttpPost]
        public IActionResult UpdateBase(float Base, int AgentSupervisorId)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            var query = (from agent_supervisor in _context.AgentSupervisor
                         where agent_supervisor.Id == AgentSupervisorId
                         select new AgentHasSupervisor
                         {
                             Id = agent_supervisor.Id,
                             Created_at = agent_supervisor.Created_at,
                             IdAgent = agent_supervisor.Id,
                             IdSupervisor = agent_supervisor.Id,
                             IdWallet = agent_supervisor.Id,
                             Base = Base,
                         }).FirstOrDefault();
            _context.AgentSupervisor.Update(query);
            _context.SaveChanges();

            return RedirectToAction("AsignarBase");
        }

    }
}

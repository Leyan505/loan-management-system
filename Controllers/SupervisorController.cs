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
                         where agent_supervisor.IdSupervisor == supervisorId
                        join users in _context.Users on agent_supervisor.IdAgent equals users.Id
                        join wallet in _context.Wallets on agent_supervisor.IdWallet equals wallet.Id
                        select new SupervisorHasAgentDTO 
                        { 
                            user = users,
                            wallet_name = wallet.Name,
                            base_total = agent_supervisor.Base
                        }).ToList();

            return View(query);
        }
        [HttpGet]
        public IActionResult UpdateBase(int Id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            var query = _context.Users.Where(x => x.Id == Id)
                .Join(_context.AgentSupervisor, user => user.Id, agentSup => agentSup.IdAgent,
                (user, agentSup) => new { user, agentSup })
                .Join(_context.Wallets, agentSup1 => agentSup1.agentSup.IdWallet, wallet => wallet.Id
                , (agentSup1, wallet) => new { agentSup1, wallet })
                .Select(result => new AssignBaseDTO()
                {
                    users = result.agentSup1.user,
                    wallet_name = result.wallet.Name,
                    base_current = result.agentSup1.agentSup.Base,
                }).FirstOrDefault();

            return View(query);
        }
        [HttpPost]
        public IActionResult UpdateBase(float Base, int AgentSupervisorId)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            var query = (from agent_supervisor in _context.AgentSupervisor
                         where agent_supervisor.IdAgent == AgentSupervisorId
                         && agent_supervisor.IdSupervisor == CurrentUser.Id
                         select new SupervisorHasAgent
                         {
                             Id = agent_supervisor.Id,
                             Created_at = agent_supervisor.Created_at,
                             IdAgent = agent_supervisor.IdAgent,
                             IdSupervisor = agent_supervisor.IdSupervisor,
                             IdWallet = agent_supervisor.IdWallet,
                             Base = Base+agent_supervisor.Base,
                         }).FirstOrDefault();
            _context.AgentSupervisor.Update(query);
            _context.SaveChanges();

            return RedirectToAction("AsignarBase");
        }

    }
}

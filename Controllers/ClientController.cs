using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;
using System.Diagnostics.Metrics;

namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "SupervisorOnly")]
    public class ClientController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? errorClient)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            var wallet = (from supag in _context.AgentSupervisor
                          where supag.IdSupervisor == CurrentUser.Id
                          join wall in _context.Wallets on supag.IdWallet equals wall.Id
                          select new WalletDTO { id = wall.Id, name = wall.Name }).ToList();

            var agents = (from supag in _context.AgentSupervisor
                          where supag.IdSupervisor == CurrentUser.Id
                          join usr in _context.Users on supag.IdAgent equals usr.Id
                          select new agentDTO { id = usr.Id, name = usr.Name, last_name = usr.LastName }).ToList();
            var countries = (from country in _context.Countries
                             select country).ToList();

            var data = new ClientEditDTO
            {
                wallets = wallet,
                agents = agents,
                countries = countries,
            };

            if (errorClient != null) { data.error = new ErrorViewModel() { description = errorClient }; }
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(int wallet, int agent, int country)
        {
            CurrentUser = new LoggedUser(HttpContext);

            if (wallet == 0)
            {
                return RedirectToAction("Index", new {
                    errorClient = "Opcion de cartera vacia"});
            }
            if (agent == 0)
            {
                return RedirectToAction("Index", new
                {
                    errorClient = "Opcion de agente vacia"
                });
            }
            if (country == 0)
            {
                return RedirectToAction("Index", new
                {
                    errorClient = "Opcion de pais vacia"
                });
            }

            _context.AgentSupervisor.Where(x => x.IdAgent == agent && x.IdSupervisor == CurrentUser.Id)
                .ExecuteUpdate(x => x.SetProperty(x => x.IdWallet, wallet));


            return RedirectToAction("Index");

        }

        public IActionResult Show(int id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            List<ClientDTO> data = (from client in _context.AgentHasClient
                                    where client.Id_wallet == id
                                    join users in _context.Users on client.Id_client equals users.Id
                                    select new ClientDTO()
                                    {
                                        name = users.Name,
                                        last_name = users.LastName,
                                        province = users.Province,
                                        status = users.Status,
                                        Id = users.Id,
                                    }).ToList();
            foreach (ClientDTO datum in data)
            {
                datum.inprogress = (from credit in _context.Credit
                                    where credit.Status == "inprogress" && credit.Id_user == datum.Id
                                    select credit).Count();
                datum.closed = (from credit in _context.Credit
                                where credit.Status == "close" && credit.Id_user == datum.Id
                                select credit).Count();
                datum.total_credit = (from credit in _context.Credit
                                      where credit.Id_user == datum.Id
                                      select credit).Count();
            }


            return View(data);
        }

        public IActionResult Edit(int id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            UsersDTO? data = (from user in _context.Users
                              where user.Id == id
                              select new UsersDTO()
                              {
                                  Address = user.Address,
                                  City = user.City,
                                  Country = user.Country,
                                  Name = user.Name,
                                  LastName = user.LastName,
                                  Phone = user.Phone,
                                  lat = user.lat,
                                  lng = user.lng,
                                  Status = user.Status,
                                  Nit = user.Nit,
                                  Province = user.Province,
                                  Id = id
                              }).FirstOrDefault();

            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(UsersDTO user, int id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            _context.Users.Where(x => x.Id == id).ExecuteUpdate(x => x.SetProperty(x => x.Address, user.Address)
            .SetProperty(x => x.Province, user.Province)
            .SetProperty(x => x.Nit, user.Nit)
            .SetProperty(x => x.Name, user.Name)
            .SetProperty(x => x.LastName, user.LastName)
            .SetProperty(x => x.lat, user.lat)
            .SetProperty(x => x.lng, user.lng)
            .SetProperty(x => x.Status, user.Status));


            if (_context.AgentHasClient.Where(x => x.Id_client == id).Any())
            {
                AgentHasClient wallet = _context.AgentHasClient.Where(x => x.Id_client == id).FirstOrDefault();
                return RedirectToAction("Show", new {id = wallet.Id});
            }
            return RedirectToAction("Index");
        }
    }
}

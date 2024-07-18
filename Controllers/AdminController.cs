using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;
using System.Diagnostics.Contracts;
using System.Security.Claims;

namespace PrestamosCreciendo.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private LoggedUser CurrentUser;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult Index()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;
            return View();
        }


        [HttpGet]
        public IActionResult CreateUsers() {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsers(Users user)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            } 
            else
            {
                return View(new ErrorViewModel { description = "Valores invalidos" });
            }
            return RedirectToAction("ManageUsers", "Admin");
        }


        [HttpGet]
        public IActionResult CreateWallet()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet(Wallet wallet)
        {

            if (ModelState.IsValid)
            {
                _context.Wallets.Add(wallet);
                _context.SaveChanges();
            }
            else
            {
                return View(new ErrorViewModel { description = "Valores invalidos" });
            }
            return RedirectToAction("ManageUsers", "Admin");
        }

        [HttpGet]
        public IActionResult ManageUsers()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            UsersList usersList = new UsersList();

            usersList._users = (from user in _context.Users 
                               select new Users
                               {
                                   Id = user.Id,
                                   Email = user.Email,
                                   Name = user.Name,
                                   Level = user.Level,
                                   Password = user.Password
                               }).ToList();
            

            return View(usersList);
        }


        [HttpPost]
        public IActionResult ManageUsers(int Id, string Action)
        {

            if (Action.Equals("Edit"))
            {
                return RedirectToAction("EditUsers", "Admin", new { Id = Id });
            }
            if(Action.Equals("Delete"))
            {
                var User = (from user in _context.Users
                            where user.Id == Id
                            select user).FirstOrDefault();
     
                _context.Users.Remove(User);
                _context.SaveChanges();
            }
            if (Action.Equals("Asignar"))
            {
                return RedirectToAction("AssignAgent", "Admin", new { Id = Id });

            }
            return RedirectToAction("ManageUsers", "Admin");

        }

        [HttpGet]
        public IActionResult EditUsers(int Id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            var User = (from user in _context.Users
                       where user.Id == Id
                         select new Users()
                       {
                           Email = user.Email,
                           Name = user.Name,
                           Level = user.Level,
                           Password = user.Password
                       }).FirstOrDefault();
            return View(User);
        }

        [HttpPost]
        public IActionResult EditUsers(Users user)
        {

            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageUsers", "Admin");
        }

        [HttpGet]
        public IActionResult ManageWallets()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            WalletsList walletList = new WalletsList();

            walletList._wallets = (from Wallet in _context.Wallets
                                select new Wallet
                                {
                                    Id = Wallet.Id,
                                    Name = Wallet.Name,
                                    City = Wallet.City,
                                    Created_at = Wallet.Created_at
                                }).ToList();


            return View(walletList);
        }


        [HttpPost]
        public IActionResult ManageWallets(int Id, string Action)
        {
            if (Action.Equals("Edit"))
            {
                return RedirectToAction("EditWallet", "Admin", new { Id = Id });
            }
            if (Action.Equals("Delete"))
            {
                var Wallet = (from wallet in _context.Wallets
                            where wallet.Id == Id
                            select wallet).FirstOrDefault();

                _context.Wallets.Remove(Wallet);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageWallets", "Admin");

        }

        [HttpGet]
        public IActionResult EditWallet(int Id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            var Wallet = (from wallet in _context.Wallets
                        where wallet.Id == Id
                        select new Wallet()
                        {
                            Id = wallet.Id,
                            Name = wallet.Name,
                            City = wallet.City,
                            Created_at = wallet.Created_at
                        }).FirstOrDefault();
            return View(Wallet);
        }

        [HttpPost]
        public IActionResult EditWallet(Wallet wallet)
        {

            if (ModelState.IsValid)
            {
                _context.Wallets.Update(wallet);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageWallets", "Admin");
        }

        [HttpGet]
        public IActionResult AssignAgent(int Id)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Level"] = CurrentUser.Level;

            AgentsDTO Agents = new AgentsDTO()
            {
                AgentsList = (from user in _context.Users
                         where user.Level == "agente"
                         select new Users()
                         {
                             Name = user.Name,
                             Email = user.Email,
                             Password = user.Password,
                             Level = user.Level,
                             Id = user.Id
                         }).ToList(),
                WalletsList = (from wallet in _context.Wallets
                               select wallet).ToList(),
                SupervisorId = Id

            };

            

            return View(Agents);
        }

        [HttpPost]
        public IActionResult AssignAgent(int IdAgent, int IdSupervisor, int IdWallet)
        {
            AgentHasSupervisor relation = new AgentHasSupervisor()
            {
                IdAgent = IdAgent,
                IdSupervisor = IdSupervisor,
                IdWallet = IdWallet,
                Base = 0,
            };

            _context.AgentSupervisor.Add(relation);
            _context.SaveChanges();

            return RedirectToAction("ManageUsers", "Admin");
        }
    }
}

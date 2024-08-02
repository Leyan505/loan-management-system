using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;
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

        public static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        }

        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            RandomNumberGenerator.Create().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }

        [HttpGet]
        public IActionResult Index()
        {

            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;
            float base_total = (from agsup in _context.AgentSupervisor
                                select agsup.Base).Sum();

            float total_bill = (from bill in _context.Bills
                                select bill.Amount).Sum();
            float total_summary = (from summary in _context.Summary
                                   select summary.Amount).Sum();
            float total_credit = (from credit in _context.Credit
                                  select credit.Amount_neto).Sum();


            return View(new AdminResumeDTO()
            {
                base_total = base_total,
                total_bill = total_bill,    
                total_summary = total_summary,
                total_credit = total_credit,
            });
        }


        [HttpGet]
        public IActionResult CreateUsers() {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsers(Users user)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            if (ModelState.IsValid && !_context.Users.Where(x => x.Email == user.Email).Any())
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            else if (_context.Users.Where(x => x.Email == user.Email).Any())
            {
                return View(new UsersDTO() { Error = new ErrorViewModel() { description = "El username ya existe" } });
            }
            else
            {
                return View(new UsersDTO() { Error = new ErrorViewModel() { description = "Valores invalidos" } });
            }
            return RedirectToAction("ManageUsers", "Admin");
        }


        [HttpGet]
        public IActionResult CreateWallet()
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

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
            ViewData["Name"] = CurrentUser.Name;

            List<UsersList> usersList;

            usersList = (from user in _context.Users
                         join supervisor in _context.AgentSupervisor on
                         user.Id equals supervisor.IdAgent into sg
                         from sup in sg.DefaultIfEmpty()
                         join wallet in _context.Wallets 
                         on sup.IdWallet equals wallet.Id into wg
                         from wall in wg.DefaultIfEmpty()
                         select new UsersList()
                         {
                             Id = user.Id,
                             Email = user.Email,
                             Name = user.Name,
                             Level = user.Level,
                             WalletName = wall.Name ?? string.Empty,
                             SupervisorName = _context.Users.Where(x => x.Id == sup.IdSupervisor).FirstOrDefault().Name ?? string.Empty,
                             ActiveUser = user.ActiveUser


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

                User.ActiveUser = false;
                User.Password = GetRandomAlphanumericString(8);
                if (User.Level == "user")
                {
                    /*Devolver credito prestado*/
                    int id_credit = _context.Credit.Where(x => x.Id_user == User.Id).FirstOrDefault().Id;

                    _context.Credit.Remove(_context.Credit.Where(x => x.Id == id_credit).FirstOrDefault());
                    _context.Users.Remove(User);
                }
                else
                {
                    _context.Users.Update(User);
                }
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
            ViewData["Name"] = CurrentUser.Name;

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
            ViewData["Name"] = CurrentUser.Name;

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
            ViewData["Name"] = CurrentUser.Name;

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
        public IActionResult AssignAgent(int Id, string? ErrorMSJ)
        {
            CurrentUser = new LoggedUser(HttpContext);
            ViewData["Name"] = CurrentUser.Name;

            AgentsDTO Agents = new AgentsDTO()
            {
                AgentsList = (from user in _context.Users
                              where user.Level == "agente"
                              select new UsersAssignDTO()
                              {
                                  Name = user.Name,
                                  Email = user.Email,
                                  Password = user.Password,
                                  Level = user.Level,
                                  Id = user.Id,
                                  ocuped = _context.AgentSupervisor.Where(x => x.IdAgent == user.Id).Any()
                              }).ToList(),
                WalletsList = (from wallet in _context.Wallets
                               select new WalletAssignDTO()
                               {
                                   City = wallet.City,
                                   Country = wallet.Country,
                                   Created_at= wallet.Created_at,
                                   Id = wallet.Id,
                                   Name = wallet.Name,
                                   ocuped = false
                               }).ToList(),
                SupervisorId = Id

            };
            if (ErrorMSJ != null)
            {
                Agents.Error = new ErrorViewModel() { description = ErrorMSJ };
            }



            return View(Agents);
        }

        [HttpPost]
        public IActionResult AssignAgent(int IdAgent, int IdSupervisor, int IdWallet)
        {
            if(_context.AgentSupervisor.
                Where(x => x.IdAgent == IdAgent && x.IdSupervisor == IdSupervisor)
                .Any())
            {
                return RedirectToAction("AssignAgent", "Admin", new {id = IdSupervisor, ErrorMSJ = "Ya está asignado a este agente"});
            }
            if(IdAgent == 0)
            {
                return RedirectToAction("AssignAgent", "Admin", new { id = IdSupervisor, ErrorMSJ = "Ya no hay agentes disponibles" });
            }
            SupervisorHasAgent relation = new SupervisorHasAgent()
            {
                IdAgent = IdAgent,
                IdSupervisor = IdSupervisor,
                IdWallet = IdWallet,
                Base = 0,
                Created_at = DateTime.UtcNow
            };

            _context.AgentSupervisor.Add(relation);
            _context.SaveChanges();

            return RedirectToAction("ManageUsers", "Admin");
        }
    }
}

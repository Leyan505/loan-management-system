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

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            } 
            else
            {
                return View(new ErrorViewModel { description = "Valores invalidos" });
            }
            return View();
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
            return View();
        }
    }
}

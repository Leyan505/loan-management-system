using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrestamosCreciendo.Data;
using PrestamosCreciendo.Models;

namespace PrestamosCreciendo.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private string? ReturnUrl;


        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            ClaimsPrincipal c = HttpContext.User;
            if (c.Identity != null)
            {
                if (c.Identity.IsAuthenticated)
                {
                    string level = c.Claims.ElementAt(2).Value;
                    if (level == "admin") 
                        return RedirectToAction("Index", "Admin");
                    if (level == "supervisor")
                        return RedirectToAction("Index", "Home");
                    if (level == "agente")
                        return RedirectToAction("Index", "Home");
                }
            }
            return View(new ErrorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string Password, string Remember = "False", string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            var user = await AuthenticateUser(email, Password);
            if (user == null)
            {
                return View(new ErrorViewModel { description = "Credenciales incorrectas"});
            }
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("FullName", user.Name),
            new Claim(ClaimTypes.Role, user.Level),
        };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var authProperties = new AuthenticationProperties();
            
            if (Remember.Equals("True"))
            {
                authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.


                    IsPersistent = true
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };
            }
            

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string User, string Password, string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ReturnUrl = returnUrl;
            return RedirectToAction("Login", "Login");
        }

        public async Task<Users> AuthenticateUser(string email, string Password)
        {
            var query = (from user in _context.Users
                         where user.Password == Password
                         && user.Email == email 
                         select user).ToList();
            if (!query.Any())
            {
                return null;
            }

            Users User = new Users
            {
                Name = query[0].Name,
                Email = query[0].Email,
                Password = query[0].Password,
                Level = query[0].Level
            };

            return User;
        }
    }
}


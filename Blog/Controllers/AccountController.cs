using Blog.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Okta.Sdk;
using System.Linq;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOktaClient _oktaClient;
        private readonly BlogContext _context;

        public AccountController(BlogContext context, IOktaClient oktaClient = null)
        {
            _context = context;
            _oktaClient = oktaClient;
        }

        public IActionResult Login(string prevUrl)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }

            AddUserIfNotExists();

            if (!string.IsNullOrEmpty(prevUrl))
                return Redirect(prevUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Index", "Home");
        }

        private void AddUserIfNotExists()
        {
            var userId = User.Claims.First(c => c.Properties.Values.Contains("sub"))?.Value;
            var userName = User.Identity.Name;

            if (_context.Users.Find(userId) != null) return;
            _context.Users.Add(new Models.User() { Id = userId, Name = userName});
            _context.SaveChanges();

        }

    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/admin/twitter"
            }, TwitterDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        public async Task<IActionResult> Twitter()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(TwitterDefaults.AuthenticationScheme);

            var userId = ulong.Parse(authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != 14201790)
            {
                return Redirect("/admin/logout");
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticateResult.Principal);
            return Redirect("/admin/");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
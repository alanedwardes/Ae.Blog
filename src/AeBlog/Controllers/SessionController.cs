using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AeBlog.Controllers
{
    public class SessionController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login() => Challenge(new AuthenticationProperties
        {
            RedirectUri = "/session/twitter"
        }, TwitterDefaults.AuthenticationScheme);

        public IActionResult Denied() => Content("Unathorized");

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [Authorize]
        public async Task<IActionResult> Twitter()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(TwitterDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticateResult.Principal);
            return Redirect("/");
        }
    }
}
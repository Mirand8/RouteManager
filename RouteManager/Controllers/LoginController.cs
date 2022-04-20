using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RouteManager.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RouteManager.Controllers
{
    public class LoginController : Controller
    {
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;

        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Required][EmailAddress] string email, [Required] string password, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                    if (result.Succeeded) return Redirect(returnUrl ?? "/");
                }
                ModelState.AddModelError(nameof(email), "Falha no login! Email ou senha invalidos!");
            }
            return View();
        }

        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

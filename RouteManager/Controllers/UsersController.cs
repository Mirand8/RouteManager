using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RouteManager.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RouteManager.Controllers
{
    public class UsersController : Controller
    {
        readonly UserManager<AppUser> _userManager;
        readonly RoleManager<AppRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                var appUser = new AppUser { UserName = user.Name, Email = user.Email };

                var result = await _userManager.CreateAsync(appUser, user.Password);
                if (!result.Succeeded)
                    foreach (IdentityError error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                ViewBag.Message = "Usuario criado com sucesso!";
            }

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync( new AppRole() { Name = userRole.RoleName });
                if (!result.Succeeded) result.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));

                ViewBag.Message = "Role criada com sucesso!";
            }

            return View();
        }
    }
}

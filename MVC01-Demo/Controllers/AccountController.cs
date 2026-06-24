using GymManagement.BLL.ViewModels.AccountViewModels;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MVC01_Demo.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager
                                 )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // get :: login empty form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // post for signin
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if(result.IsLockedOut)
            {
                return View(model);

            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(model); 
            }
        }

        // logout
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        
    }
}

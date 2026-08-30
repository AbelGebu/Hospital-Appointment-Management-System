using HospitalAppoinmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HospitalAppoinmentManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                // Default any newly registered account to the "Patient" role if unassigned
                if (user != null &&
                    !await _userManager.IsInRoleAsync(user, "Admin") &&
                    !await _userManager.IsInRoleAsync(user, "Doctor") &&
                    !await _userManager.IsInRoleAsync(user, "Patient"))
                {
                    await _userManager.AddToRoleAsync(user, "Patient");
                    await _signInManager.RefreshSignInAsync(user);
                }
            }

            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

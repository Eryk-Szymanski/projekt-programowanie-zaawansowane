using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication2.Data.Migrations;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signManager = signManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> MakeMeAdmin()
        {
            if(!_roleManager.RoleExistsAsync("Admin").Result)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if(!User.IsInRole("Admin"))
            {
                IdentityUser user = _userManager.GetUserAsync(User).Result;
                await _userManager.AddToRoleAsync(user, "Admin");
                await _signManager.SignOutAsync();
            }

            return View("Index");
        }
    }
}
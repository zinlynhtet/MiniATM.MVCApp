using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.Models;
using System.Diagnostics;
using MiniATM.MVCApp.EFDbContext;

namespace MiniATM.MVCApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("LoginData");
            if (userId == null) return Redirect("/login");
            var userData = await _context.UserData
                .FirstOrDefaultAsync(b => b.UserId == userId);

            return View(userData);
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
    }
}

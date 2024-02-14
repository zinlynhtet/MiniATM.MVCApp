using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.Models;
using System.Diagnostics;
using MiniATM.MVCApp.EFDbContext;
using Newtonsoft.Json;

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

        public IActionResult Index()
        {
            var userData = HttpContext.Session.GetString("LoginData");
            if (userData == null)
                return Redirect("/login");
            var jsonUser = JsonConvert.DeserializeObject<LoginDataModel>(userData);
            var userDetail = _context.UserData.FirstOrDefault(x => x.CardNumber == jsonUser!.CardNumber);
            return View(userDetail);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

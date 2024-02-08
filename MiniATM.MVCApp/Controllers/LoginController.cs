using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.EFDbContext;
using MiniATM.MVCApp.Models;
using Newtonsoft.Json;

namespace MiniATM.MVCApp.Controllers
{
    public class LoginController(AppDbContext  context) : Controller
    {
        private readonly AppDbContext _context = context;

        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserDataModel reqModel)
        {
            var userData = await _context.UserData
                .FirstOrDefaultAsync(b => b.CardNumber == reqModel.CardNumber && b.Password == reqModel.Password);
            if (userData == null)
            {
                return RedirectToAction("Index");
            }
            HttpContext.Session.SetString("LoginData",userData.UserId);
            return Redirect($"/home/Index");
        }
    }
}

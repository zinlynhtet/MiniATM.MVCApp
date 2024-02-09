using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.EFDbContext;
using MiniATM.MVCApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MiniATM.MVCApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserDataModel reqModel)
        {

            var userData = await _context.UserData
                .FirstOrDefaultAsync(b => b.CardNumber == reqModel.CardNumber && b.Password == reqModel.Password);

            if (userData != null)
            {
                HttpContext.Session.SetString("LoginData", userData.UserId);
                return RedirectToAction("Index", "Home");
            }
            return View(reqModel);
        }
    }
}
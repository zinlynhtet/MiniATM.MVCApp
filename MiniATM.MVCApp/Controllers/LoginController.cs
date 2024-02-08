using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.EFDbContext;
using MiniATM.MVCApp.Models;

namespace MiniATM.MVCApp.Controllers
{
    public class LoginController(AppDbContext  context) : Controller
    {
        private readonly AppDbContext _context = context;

        public IActionResult Index()
        {
            return View();
        }
    }
}

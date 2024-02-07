using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.EFDbContext;
using MiniATM.MVCApp.Models;
using NUlid;

namespace MiniATM.MVCApp.Controllers
{
    public class AdminController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        [ActionName("Index")]
        public IActionResult AdminList()
        {
            var lst = _context.AdminData.AsNoTracking().ToList();

            return View("AdminList", lst);
        }

        [ActionName("Create")]
        public IActionResult AdminRegister()
        {
            return View("AdminRegister");
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> AdminRegister(AdminDataModel reqModel)
        {
            reqModel.AdminID = Ulid.NewUlid().ToString();
            await _context.AdminData.AddAsync(reqModel);
            var adminData = await _context.SaveChangesAsync();
            var message = adminData > 0 ? "Registration Successful." : "Registration failed.";
            TempData["Message"] = message;
            TempData["IsSuccess"] = adminData > 0;
            return Redirect("/admin");
        }
    }
}

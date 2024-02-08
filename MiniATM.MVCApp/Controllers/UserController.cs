using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.EFDbContext;
using MiniATM.MVCApp.Models;
using NUlid;

namespace MiniATM.MVCApp.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var lst = _context.UserData.AsNoTracking().ToList();

            return View("UserList", lst);
        }

        [ActionName("Register")]
        public IActionResult UserRegister()
        {
            return View("UserRegister");
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> UserSave(UserDataModel reqModel)
        {

            reqModel.UserId = Ulid.NewUlid().ToString();
            reqModel.CardNumber = (int?)GenerateRandom12DigitNumber();
            await _context.UserData.AddAsync(reqModel);
            var result = await _context.SaveChangesAsync();
            var message = result > 0 ? "Registration Successful." : "Registration failed.";
            TempData["Message"] = message;
            TempData["IsSuccess"] = result > 0;

            return Redirect("/user");
        }

        private long GenerateRandom12DigitNumber()
        {
            Random random = new Random();

            long firstDigit = random.Next(1, 10);
            long remainingDigits = random.Next(0, 100000000);
            long result = firstDigit * 100000000 + remainingDigits;

            return result;
        }

        [ActionName("Withdrawal")]
        public IActionResult UserWithdrawal()
        {
            return View("UserWithdrawal");
        }
        [ActionName("Withdrawal")]
        [HttpPost]
        public async Task<IActionResult> UserWithdrawal(UserDataModel reqModel)
        {
            var userData = await _context.UserData
                .FirstOrDefaultAsync(b => b.CardNumber == reqModel.CardNumber && b.Password == reqModel.Password);

            if (userData == null || reqModel.Balance <= 0 || reqModel.Balance > userData.Balance)
            {
                TempData["Message"] = "Withdrawal failed. Invalid data or insufficient balance.";
                TempData["IsSuccess"] = false;

                return View("UserWithdrawal");
            }

            userData.Balance -= reqModel.Balance;
            _context.UserData.Update(userData);
            var result = await _context.SaveChangesAsync();
            var message = result > 0 ? $"Withdraw Successful. Remaining Balance: {userData.Balance} $" : "Withdraw failed.";
            TempData["Message"] = message;
            TempData["IsSuccess"] = result > 0;
            return Redirect("/user");
        }
    }
}

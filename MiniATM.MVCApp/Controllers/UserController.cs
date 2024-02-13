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

        [ActionName("List")]
        public async Task<IActionResult> Index(int pageNo = 1, int pageSize = 10)
        {
            UserDataResponseModel model = new UserDataResponseModel();
            List<UserDataModel> lst = _context.UserData.AsNoTracking()
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            int rowCount = await _context.UserData.CountAsync();
            int pageCount = rowCount / pageSize;
            if (rowCount % pageSize > 0)
                pageCount++;
            model.Users = lst;
            model.PageSetting = new PageSettingModel(pageNo, pageSize, pageCount, "/user/Index");
            return View("UserList", model);
        }

        [ActionName("Register")]
        public IActionResult UserRegister()
        {
            return View("UserRegister");
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> UserRegister(UserDataModel reqModel)
        {
            reqModel.UserId = Ulid.NewUlid().ToString();
            reqModel.CardNumber = (int?)GenerateRandom12DigitNumber();
            await _context.UserData.AddAsync(reqModel);
            var result = await _context.SaveChangesAsync();
            var message = result > 0 ? "Registration Successful." : "Registration failed.";
            TempData["Message"] = message;
            TempData["IsSuccess"] = result > 0;
            MessageModel model = new MessageModel(result > 0, message);
            return Json(model);
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

        [HttpPost]
        public async Task<IActionResult> UserWithdrawal(UserDataModel reqModel)
        {
            UserDataModel? user = await _context.UserData.FirstOrDefaultAsync(x => x.CardNumber == reqModel.CardNumber && x.Password == reqModel.Password);
            if (user == null || user.Balance < reqModel.Balance || reqModel.Balance == 0)
            {
                TempData["Message"] = "User not found.";
                TempData["IsSuccess"] = false;

                return Json(new ResponseMessageModel(false, "User not found."));
            }
            var transaction = _context.Database.BeginTransaction();
            try
            {
                decimal newBalance = user.Balance - reqModel.Balance;
                user.Balance = newBalance;
                _context.UserData.Update(user);
                int result = await _context.SaveChangesAsync();
                transaction.Commit();

                TempData["Message"] = "Withdrawal successful.";
                TempData["IsSuccess"] = result > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                TempData["Message"] = "An error occurred during withdrawal.";
                TempData["IsSuccess"] = false;
                throw;
            }
            return View("UserWithdrawal");
        }

        [ActionName("Deposit")]
        public IActionResult UserDeposit()
        {
            return View("UserDeposit");
        }

        [HttpPost]
        public async Task<IActionResult> UserDeposit(UserDataModel reqModel)
        {
            UserDataModel? user = await _context.UserData.FirstOrDefaultAsync(x => x.CardNumber == reqModel.CardNumber && x.Password == reqModel.Password);
            if (user == null)
            {
                TempData["Message"] = "User not found.";
                TempData["IsSuccess"] = false;

                return Json(new ResponseMessageModel(false, "User not found."));
            }
            var transaction = _context.Database.BeginTransaction();
            try
            {
                decimal newBalance = user.Balance + reqModel.Balance;
                user.Balance = newBalance;
                _context.UserData.Update(user);
                int result = await _context.SaveChangesAsync();
                transaction.Commit();
                TempData["Message"] = "Deposit successful.";
                TempData["IsSuccess"] = result > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
                TempData["Message"] = "An error occurred during deposit.";
                TempData["IsSuccess"] = false;
                throw;
            }
            return View("UserDeposit");
        }
    }
}
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

        [HttpPost]
        [ActionName("Withdrawal")]
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

        [ActionName("Deposit")]
        public IActionResult UserDeposit()
        {
            return View("UserDeposit");
        }

        [HttpPost]
        [ActionName("Deposit")]
        public async Task<IActionResult> UserDeposit(UserDataModel reqModel)
        {
            var userData = await _context.UserData
                .FirstOrDefaultAsync(b => b.CardNumber == reqModel.CardNumber && b.Password == reqModel.Password);

            if (userData == null)
            {
                TempData["Message"] = "Invalid cardNum or pin.";
                TempData["IsSuccess"] = false;
                return View("UserDeposit");
            }

            userData.Balance += reqModel.Balance;
            _context.UserData.Update(userData);
            var result = await _context.SaveChangesAsync();
            var message = result > 0 ? $"Deposit Successful. New Balance: {userData.Balance} $" : "Deposit failed.";
            TempData["Message"] = message;
            TempData["IsSuccess"] = result > 0;
            return Redirect("/user");
        }

        [HttpPost]
        [ActionName("Remove")]
        public async Task<IActionResult> RemoveUser(UserDataModel reqModel)
        {
            var userData = await _context.UserData.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == reqModel.UserId);
            if (userData is null)
            {
                TempData["Message"] = "No data found.";
                TempData["IsSuccess"] = false;
                goto result;
            }
            _context.Remove(userData);
            int result = _context.SaveChanges();
            string message = result > 0 ? "Deleting Successful." : "Deleting Failed.";
            TempData["Message"] = message;
            TempData["IsSuccess"] = result > 0;
        result:
            return Redirect("/user");
        }

        [HttpGet]
        public async Task<IActionResult> ThreeYearsTransitionRecord()
        {
            int total = 0;
            DateTime currentDate = DateTime.Now;
            DateTime addYear;
            List<int> years = new List<int>();
            var userId = HttpContext.Session.GetString("LoginData");
            var userData = await _context.UserData.FirstOrDefaultAsync(x => x.UserId == userId);
            for (int i = 0; i < 3; i++)
            {
                addYear = currentDate.AddYears(-1 * i);
                years.Add(addYear.Year);
            }
            Random random = new Random();
            int totalTransitionAmount = 0;
            bool isSuccess = true;
            using (var transition = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 365; j++)
                        {
                            total = i;
                            decimal Amount = random.Next(100000, 1000000);
                            decimal income_amount = random.Next(100000, 1000000);

                            DateTime start_date = new DateTime(years[i], 1, 1);
                            DateTime end_date = new DateTime(years[i], 12, 31);
                            int range = (end_date - start_date).Days;
                            int randomDays = random.Next(range);
                            DateTime randomDate = start_date.AddDays(randomDays);
                            string transferId = Ulid.NewUlid().ToString();
                            int transitionAmount = random.Next(1, 101);
                            totalTransitionAmount += transitionAmount;
                            var transitionHistory = new TransitionHistoryModel
                            {
                                TransferId = transferId,
                                FromAccount = userData!.CardNumber.ToString()!,
                                ToAccount = GenerateRandom12DigitNumber().ToString(),
                                TransferDate = randomDate,
                                UserId = userId!,
                                TransitionAmount = transitionAmount,
                            };
                            await _context.TransitionHistory.AddAsync(transitionHistory);
                            await _context.SaveChangesAsync();
                        }
                    }

                    if (userData!.Balance < totalTransitionAmount)
                    {
                        await transition.RollbackAsync();
                        goto result;
                    }
                    userData.Balance -= totalTransitionAmount;
                    _context.UserData.Update(userData);
                    await _context.SaveChangesAsync();
                    await transition.CommitAsync();
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    await transition.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }
        result:
            return Json(new
            {
                isSuccess = isSuccess,
                CurrentBalance = userData!.Balance,
                TransitionAmount = totalTransitionAmount,
                RemainingBalance = userData.Balance - totalTransitionAmount,
                TransitionStatus = isSuccess ? "Transition Successful." : "Transition Failed.",
            });
        }
    }
}

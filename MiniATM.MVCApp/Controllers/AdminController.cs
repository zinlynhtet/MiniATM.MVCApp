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

        [ActionName("List")]
        public async Task<IActionResult> Index(int pageNo = 1, int pageSize = 10)
        {
            AdminDataResponseModel model = new AdminDataResponseModel();
            List<AdminDataModel> lst = _context.AdminData.AsNoTracking()
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int rowCount = await _context.AdminData.CountAsync();
            int pageCount = rowCount / pageSize;
            if (rowCount % pageSize > 0)
                pageCount++;

            model.AdminData = lst;
            model.PageSetting = new PageSettingModel(pageNo, pageSize, pageCount, "/admin/adminlist");

            return View("AdminList", model);
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
            return Redirect("/admin/list");
        }

        [HttpGet]
        public async Task<IActionResult> ThreeYearsTransactionRecord()
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
            int totalTransactionAmount = 0;
            bool isSuccess = true;
            using (var transaction = await _context.Database.BeginTransactionAsync())
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
                            int transactionAmount = random.Next(1, 101);
                            totalTransactionAmount += transactionAmount;
                            var transactionHistory = new TransactionHistoryModel()
                            {
                                TransferId = transferId,
                                FromAccount = userData!.CardNumber.ToString()!,
                                ToAccount = GenerateRandom12DigitNumber().ToString(),
                                TransferDate = randomDate,
                                UserId = userId!,
                                TransitionAmount = transactionAmount,
                            };
                            await _context.TransactionHistory.AddAsync(transactionHistory);
                            await _context.SaveChangesAsync();
                        }
                    }

                    if (userData!.Balance < totalTransactionAmount)
                    {
                        await transaction.RollbackAsync();
                        goto result;
                    }

                    userData.Balance -= totalTransactionAmount;
                    _context.UserData.Update(userData);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                }
            }

            result:
            return Json(new
            {
                isSuccess,
                CurrentBalance = userData!.Balance,
                TransitionAmount = totalTransactionAmount,
                RemainingBalance = userData.Balance - totalTransactionAmount,
                TransitionStatus = isSuccess ? "Transition Successful." : "Transition Failed.",
            });
        }

        private long GenerateRandom12DigitNumber()
        {
            Random random = new Random();

            long firstDigit = random.Next(1, 10);
            long remainingDigits = random.Next(0, 100000000);
            long result = firstDigit * 100000000 + remainingDigits;

            return result;
        }

        [HttpPost]
        [ActionName("RemoveUser")]
        public async Task<IActionResult> RemoveUser(UserDataModel reqModel)
        {
            UserDataModel? userData = await _context.UserData.FirstOrDefaultAsync(x => x.UserId == reqModel.UserId);

            if (userData is null)
            {
                return Json(new MessageModel(false, "No data found."));
            }

            _context.UserData.Remove(userData);
            var result = await _context.SaveChangesAsync();
            string message = result > 0 ? "User has been deleted." : "Deleting Failed.";
            TempData["Message"] = message;
            TempData["IsSuccess"] = result > 0;

            MessageModel model = new MessageModel(result > 0, message);
            return Json(model);
        }
    }
}
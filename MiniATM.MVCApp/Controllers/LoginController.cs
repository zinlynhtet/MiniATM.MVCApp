using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniATM.MVCApp.EFDbContext;
using MiniATM.MVCApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MiniATM.MVCApp.Services;
using Newtonsoft.Json;

namespace MiniATM.MVCApp.Controllers
{
    //public class LoginController : Controller
    //{
    //    private readonly AppDbContext _context;

    //    public LoginController(AppDbContext context)
    //    {
    //        _context = context;
    //    }

    //    public IActionResult Index()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Index(UserDataModel reqModel)
    //    {

    //        var userData = await _context.UserData
    //            .FirstOrDefaultAsync(b => b.CardNumber == reqModel.CardNumber && b.Password == reqModel.Password);

    //        if (userData != null)
    //        {
    //            HttpContext.Session.SetString("LoginData", userData.UserId);
    //            return RedirectToAction("Index", "Home");
    //        }
    //        return View(reqModel);
    //    }

    //    public IActionResult AdminLogin()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> AdminLogin(AdminDataModel reqModel)
    //    {
    //        var adminData = await _context.AdminData
    //            .FirstOrDefaultAsync(x => x.AdminUsername == reqModel.AdminUsername && x.AdminPassword == reqModel.AdminPassword);

    //        if (adminData != null)
    //        {
    //            HttpContext.Session.SetString("LoginData", adminData.AdminID);
    //            return RedirectToAction("Index", "User");
    //        }

    //        return View(reqModel);
    //    }
    ////}
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly MenuService _menuService;

        public LoginController(IConfiguration configuration, AppDbContext context, MenuService menuService)
        {
            _configuration = configuration;
            _context = context;
            _menuService = menuService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [ActionName("AdminLogin")]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginDataModel reqModel)
        {
            if (reqModel is null)
            {
                return View();
            }
            var user = AuthenticateUser(reqModel);

            if (user != null)
            {
                var tokenString = GenerateJwtToken(user);
                var option = new CookieOptions();
                option.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Append("Authorization", tokenString, option);
            }
            return Redirect("/Home");
        }

        private AuthenticationModel AuthenticateUser(LoginDataModel reqModel)
        {
            var roleData = new AuthenticationModel();
            var userData = _context.UserData
                       .FirstOrDefault(b => b.CardNumber == reqModel.CardNumber && b.Password == reqModel.Password);
            var adminData = _context.AdminData
                       .FirstOrDefault(x => x.AdminUsername == reqModel.AdminUsername && x.AdminPassword == reqModel.AdminPassword);
            if (userData is not null)
            {
                roleData.Role = "User";
                var userMenu = _menuService.GenerateMenu(roleData.Role);
                HttpContext.Session.SetString("Menu",JsonConvert.SerializeObject(userMenu));
            }

            if (adminData is not null)
            {
                roleData.Role = "Admin";
                var adminMenu = _menuService.GenerateMenu(roleData.Role);
                HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(adminMenu));
            }
            return roleData;
        }

        private string GenerateJwtToken(AuthenticationModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, "Admin"),
                new(ClaimTypes.Role, "User"),
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
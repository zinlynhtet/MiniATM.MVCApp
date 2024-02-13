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

        [ActionName("Index")]
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
            var tokenString = GenerateJwtToken(user);
            var option = new CookieOptions();
            option.Expires = DateTime.Now.AddMinutes(30);
            Response.Cookies.Append("Authorization", tokenString, option);

            if (user.Role is "Admin") goto result;
            if (user.Role is "User")
            {
                return Redirect("/Home");
            }
            result:
            return Redirect("/user/list");
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
                HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(userMenu));
                HttpContext.Session.SetString("LoginData", JsonConvert.SerializeObject(reqModel));
            }

            if (adminData is not null)
            {
                roleData.Role = "Admin";
                var adminMenu = _menuService.GenerateMenu(roleData.Role);
                HttpContext.Session.SetString("Menu", JsonConvert.SerializeObject(adminMenu));
                HttpContext.Session.SetString("AdminData", JsonConvert.SerializeObject(reqModel));
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
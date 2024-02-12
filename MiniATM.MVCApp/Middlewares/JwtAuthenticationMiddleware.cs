using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MiniATM.MVCApp.Middlewares;

public class JwtAuthenticationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path == "/" ||
            context.Request.Path.ToString().ToLower() == "/login" ||
            context.Request.Path.ToString().ToLower() == "/login/index")
        {
            await next.Invoke(context);
            return;
        }

        if (context.Request.Cookies.ContainsKey("Authorization"))
        {
            var jwtToken = context.Request.Cookies["Authorization"]!;
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(jwtToken);
            var identity = new ClaimsIdentity(decodedToken.Claims);
            context.User.AddIdentity(identity);

            await next.Invoke(context);
            return;
        }

        context.Response.Redirect("/login");
    }
}
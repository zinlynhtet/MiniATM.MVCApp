namespace MiniATM.MVCApp.Models;

public class LoginDataModel
{
    public string AdminUsername { get; set; }
    public string AdminPassword { get; set; }
    public string UserId { get; set; }
    public int? CardNumber { get; set; }
    public string Password { get; set; }
}

public class AuthenticationModel
{
    public string Role { get; set; }
}
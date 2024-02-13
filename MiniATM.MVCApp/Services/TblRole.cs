using NUlid;

namespace MiniATM.MVCApp.Services;

public class TblRole
{
    public string RoleId { get; set; }
    public string RoleCode { get; set; }
    public string RoleName { get; set; }
    public bool IsAdmin { get; set; }

    public TblRole()
    {

    }
    public TblRole(string roleCode, string roleName, bool isAdmin)
    {
        RoleId = Ulid.NewUlid().ToString();
        RoleCode = roleCode;
        RoleName = roleName;
        IsAdmin = isAdmin;
    }

    public static List<TblRole> Roles = new List<TblRole>()
    {
        new TblRole("R001","User",false),
        new TblRole("R002","Admin",true),
    };
}
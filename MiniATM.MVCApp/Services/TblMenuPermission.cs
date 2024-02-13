using NUlid;

namespace MiniATM.MVCApp.Services;

public class TblMenuPermission
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string RoleCode { get; set; }
    public string MenuCode { get; set; }

    public TblMenuPermission()
    {

    }
    public TblMenuPermission(string roleCode,string menuCode)
    {
        Id = Ulid.NewUlid().ToString();
        UserId = Ulid.NewUlid().ToString();
        MenuCode = menuCode;
        RoleCode = roleCode;
    }

    public static List<TblMenuPermission> MenuPermission = new List<TblMenuPermission>()
    {
        new TblMenuPermission("R001","M001"),
        new TblMenuPermission("R002","M002")
    };
}
using NUlid;

namespace MiniATM.MVCApp.Services;

public class TblMenuHead
{
    public string Id { get; set; }
    public string MenuCode { get; set; }
    public string MenuName { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string RoleCode { get; set; }
    public bool IsHasChild { get; set; }

    public TblMenuHead()
    {

    }
    public TblMenuHead(string menuCode, string menuName, string controllerName,string actionName,
        string roleCode, bool isHasChild)
    {
        Id = Ulid.NewUlid().ToString();
        MenuCode = menuCode;
        MenuName = menuName;
        RoleCode = roleCode;
        ControllerName = controllerName;
        IsHasChild = isHasChild;
    }

    public static List<TblMenuHead> MenuHead = new List<TblMenuHead>()
    {
        new TblMenuHead("M001","Options","","","R001",true),
        new TblMenuHead("M002","AdminOptions","","","R002",true),
    };
}
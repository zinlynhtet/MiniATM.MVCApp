using NUlid;

namespace MiniATM.MVCApp.Services;

public class TblMenuDetail
{
    public string Id { get; set; }
    public string MenuParentCode { get; set; }
    public string MenuChildCode { get; set; }
    public string MenuChildName { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public string RoleCode { get; set; }

    public TblMenuDetail()
    {

    }
    public TblMenuDetail(string menuParentCode, string menuChildCode,
        string menuChildName, string controllerName,string actionName, string? roleCode = null)
    {
        Id = Ulid.NewUlid().ToString();
        MenuParentCode = menuParentCode;
        MenuChildCode = menuChildCode;
        MenuChildName = menuChildName;
        ControllerName = controllerName;
        ActionName = actionName;
        RoleCode = roleCode;
    }

    public static List<TblMenuDetail> MenuDetail = new List<TblMenuDetail>()
    {
        new TblMenuDetail("M001","C001","Withdrawal","User","UserWithdrawal"),
        new TblMenuDetail("M001","C002","Deposit","User","Deposit"),
        new TblMenuDetail("M002","C003","User Lists","User","List"),
        new TblMenuDetail("M002","C004","Admin Lists","Admin","List"),
        new TblMenuDetail("M002","C005","User Registration","User","Register"),
        new TblMenuDetail("M002","C006","Admin Registration","Admin","Create"),
    };
}
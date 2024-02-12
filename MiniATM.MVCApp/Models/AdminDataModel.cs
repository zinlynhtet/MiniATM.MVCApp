using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniATM.MVCApp.Models;

[Table("AdminTable")]
public class AdminDataModel
{
    [Key]
    public string AdminID { get; set; }
    public string AdminUsername { get; set; }
    public string AdminPassword { get; set; }
}

public class AdminDataResponseModel
{
    public List<AdminDataModel> AdminData { get; set; }
    public PageSettingModel PageSetting { get; set; }

}

public class MessageModel
{
    public MessageModel() { }
    public MessageModel(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}

public class PageSettingModel
{
    public PageSettingModel()
    {
    }
    public PageSettingModel(int pageNo, int pageSize, int pageCount, string pageUrl)
    {
        PageNo = pageNo;
        PageSize = pageSize;
        PageCount = pageCount;
        PageUrl = pageUrl;
    }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int PageCount { get; set; }
    public string PageUrl { get; set; }
}
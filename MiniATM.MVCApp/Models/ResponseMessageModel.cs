namespace MiniATM.MVCApp.Models;

public class ResponseMessageModel
{
    public ResponseMessageModel() { }
    public ResponseMessageModel(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}
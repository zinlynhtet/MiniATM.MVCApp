using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniATM.MVCApp.Models
{
    [Table("AdminTable")]
    public class AdminDataModel
    {
        [Key]
        public string AdminID { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
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
}

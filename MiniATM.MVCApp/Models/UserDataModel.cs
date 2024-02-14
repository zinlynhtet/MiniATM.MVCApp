using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniATM.MVCApp.Models
{
    [Table("UserTable")]

    public class UserDataModel
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string NRC { get; set; }
        public int? CardNumber { get; set; }
        [Range(0, 8)]
        public string Password { get; set; }
        public decimal Balance { get; set; }
    }
    public class UserDataResponseModel
    {
        public PageSettingModel PageSetting { get; set; }
        public List<UserDataModel> Users { get; set; }
    }

    public class UserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string NRC { get; set; }
        public int? CardNumber { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
    }
}

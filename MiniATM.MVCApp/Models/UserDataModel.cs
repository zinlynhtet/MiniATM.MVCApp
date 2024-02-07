using System.ComponentModel.DataAnnotations;

namespace MiniATM.MVCApp.Models
{
    public class UserDataModel
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string NRC { get; set; }
        public int CardNumber { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
    }
}

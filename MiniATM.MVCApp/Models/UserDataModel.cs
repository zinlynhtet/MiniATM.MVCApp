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
        public int? CardNumber { get; set; } = null;
        public string Password { get; set; }
        public decimal Balance { get; set; }
    }
}

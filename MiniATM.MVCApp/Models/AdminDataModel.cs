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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniATM.MVCApp.Models
{
    [Table("Tbl_TransitionHistory")]
    public class TransitionHistoryModel
    {
        [Key]
        public int TransitionHistoryId { get; set; }
        public string TransferId { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public decimal TransitionAmount { get; set; }
        public DateTime TransferDate { get; set; }
        public string UserId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace CHKS.Models
{
    public class EmployeeModel
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<GroupModel> Group { get; set; }
        public ICollection<StockLogs> StockLogs { get; set; }
        public ICollection<TransactionModel> Transactions { get; set; }
    }
}
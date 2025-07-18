using System.ComponentModel.DataAnnotations;

namespace CHKS.Models
{
    public class EmployeeModel
    {
        [Key]
        [Required]
        public int Id { get; set; } = Random.Shared.Next();
        
        [Required]
        public string Name { get; set; }

        public ICollection<GroupModel> Group { get; set; }
        public ICollection<StockLogsModel> StockLogs { get; set; }
        public ICollection<TransactionModel> Transactions { get; set; }
    }
}
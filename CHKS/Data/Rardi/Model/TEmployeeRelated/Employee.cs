using System.ComponentModel.DataAnnotations;

namespace CHKS.Models
{
    public class Employee
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<Groups> Group { get; set; } = new List<Groups>();
        public ICollection<StockLogs> StockLogs { get; set; } = new List<StockLogs>();
    }
}
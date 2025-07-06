using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CHKS.Models.mydb;
using DocumentFormat.OpenXml.InkML;
namespace CHKS.Models;

[Table("Stock_Logs")]
public class StockLogs
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    [Required]
    public Guid ProductId { get; set; }
    public Product_Model Product { get; set; }

    [Timestamp]
    [Column("Time")]
    public DateTime Date { get; set; }
    public int Amount { get; set; } = 1;

    [Column(TypeName = "ENUM('IN', 'OUT')")]
    public TransactionLogType Type { get; set; } // "in" or "out"
    
    public bool Seen { get; set; } = false;
}
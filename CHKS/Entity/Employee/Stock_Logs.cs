
namespace CHKS.Entity;

using Enum;
public class StockLogs
{
    public Guid Id { get; set; }
    public int EmployeeId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime Date { get; set; }
    public int Amount { get; set; }
    public TransactionLogType Type { get; set; } // "in" or "out"

}
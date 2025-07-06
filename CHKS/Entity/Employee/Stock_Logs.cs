
namespace CHKS.Entity;

public class StockLogs
{
    public Guid Id { get; set; }
    public int EmployeeId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime Date { get; set; }
    public int Amount { get; set; }
    public TransactionLogType Type { get; set; } // "in" or "out"

    public static StockLogs FromModelStockLogs(ref Models.StockLogs stockLogs) => new()
    {
        Id = stockLogs.Id,
        EmployeeId = stockLogs.EmployeeId,
        ProductId = stockLogs.ProductId,
        Date = stockLogs.Date,
        Amount = stockLogs.Amount,
        Type = TransactionLogType.Out
    };
}
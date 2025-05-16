using CHKS.Entity;

public class Transaction
{
    public Guid Id { get; set; }
    public Customer Customer { get; set; }
    public decimal Total { get; set; } = 0.0m;
    public short Status { get; set; } = 0;
    public List<TransactionItem> TransactionContent { get; set; } = new List<TransactionItem>();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}
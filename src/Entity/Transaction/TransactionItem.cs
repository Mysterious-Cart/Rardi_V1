using CHKS.Entity;
public class TransactionItem
{
    public Guid Id { get; set; }
    public Transaction Transaction { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; } = 0.0m;
    public decimal GetTotalPrice() => Price * Quantity;
    public DateOnly? GuaranteeDuration { get; set; } = null;

}
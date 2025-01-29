namespace CHKS.Models.Interface;
using CHKS.Models.mydb;

public interface ICartItem
{
    public int CartId { get; set; }

    public Guid ProductId { get; set; }

    public Inventory Inventory { get; }

    public decimal Qty { get; set; }

    public string Note { get; set; }

    public Guid Id { get; }

    public decimal? PriceOverwrite { get; set; }
}
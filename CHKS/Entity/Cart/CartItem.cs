using CHKS.Extras.Class.DTOs;
using CHKS.Entity;
public class CartItem(Guid Id, string name, int quantity, decimal unit_price, decimal price)
{
    public Guid ProductId { get; set; } = Id;
    public string Name { get; set; } = name;
    public int Quantity { get; set; } = quantity;
    public decimal UnitPrice { get; set; } = unit_price; //Product's original price
    public decimal Price { get; set; } = price; // Sale price or set price
    public decimal GetTotalPrice() => Price * Quantity;
}
using CHKS.Extras.Class.DTOs;
using CHKS.Entity;
using DocumentFormat.OpenXml.Bibliography;
public class CartItem(Guid Id, string name, int quantity, decimal unit_price, decimal price)
{
    public Guid Id { get; set; } = Id;
    public string Name { get; set; } = name;
    public int Quantity { get; set; } = quantity;
    public decimal UnitPrice { get; set; } = unit_price;
    public decimal Price { get; set; } = price;
    public decimal GetTotalPrice() => Price * Quantity;
}
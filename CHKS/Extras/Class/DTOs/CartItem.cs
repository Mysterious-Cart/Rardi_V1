using CHKS.Extras.Class.DTOs;
using CHKS.Entity;

public class CartItemDTO()
{
    public Guid Id { get; set; }
    public int CartId { get; set; }
    public CartDTO Cart { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Original_Price { get; set; }
    public decimal? Set_Price { get; set; }
    public string Note { get; set; }

}
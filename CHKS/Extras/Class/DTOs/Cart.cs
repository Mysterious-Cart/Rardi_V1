using CHKS.Entity;
using CHKS.Extras.Class.DTOs;

public class CartDTO
{
    public int Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer customer { get; set; }
    public decimal Total { get; set; }
    private ICollection<CartItemDTO> _CartContent { get; set; }

}
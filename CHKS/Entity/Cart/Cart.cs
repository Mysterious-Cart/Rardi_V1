using System.Collections.ObjectModel;

namespace CHKS.Entity;
public class Cart(int CartId, string Plate_Numbers, decimal total, List<CartItem> CartContents)
{
    public int CartId { get; set; } = CartId;
    public decimal Total { get; set; } = total;
    public string Plate_Numbers { get; set; } = Plate_Numbers;
    public List<CartItem> CartContents { get; set; } = [];
}
using System.Net.Mime;
using System.Runtime.InteropServices;
using CHKS.Entity;

namespace CHKS.Entity;
public class Cart(int CartId, string Plate_Numbers, decimal total, IEnumerable<CartItem> contents) 
    : CartBase<CartItem>(Plate_Numbers, total, contents)
{
    public int CartId { get; set; } = CartId;
}
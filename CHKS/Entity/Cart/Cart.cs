using System.Collections.ObjectModel;

namespace CHKS.Entity;

public record Cart(int CartId, string Plate_Numbers, decimal Total, List<CartItem> CartContents);

public record CartItem(Guid ProductId, int Amount, decimal Price);
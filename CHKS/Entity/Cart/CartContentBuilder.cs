
namespace CHKS.Entity;
public class CartItemBuilder
{
    private Guid _id;
    private string _name;
    private decimal _unit_price;
    private int _quantity;
    private decimal _price;
    /*
    public static CartItemBuilder FromModel(CartItem_Model model)
    {
        CartItemBuilder.Empty()
            .WithId(model.CartItemId)
            .WithCart(CartBuilder.FromModel(model.Cart).Build())
            .WithProduct(ProductBuilder.FromModel(model.Product).Build())
        return this;
    }
    */

    public static CartItemBuilder Empty()
    {
        return new CartItemBuilder();
    }

    public CartItemBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public CartItemBuilder WithUnitPrice(decimal UnitPrice)
    {
        _unit_price = UnitPrice;
        return this;
    }
    public CartItemBuilder WithProductId(Guid id)
    {
        _id = id;
        return this;
    }

    public CartItemBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public CartItem Build()
    {
        return new CartItem(_id, _name, _quantity, _unit_price, _price);
    }
}
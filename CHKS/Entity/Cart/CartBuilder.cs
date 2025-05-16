using CHKS.Entity;
using CHKS.Models.mydb;

namespace CHKS.Entity;
public class CartBuilder : IEntityBuilder<Cart, Cart_Model>
{
    private int _cartId;
    private string _plateNumbers;
    private decimal _total;
    public static Cart_Model ToModel(Cart cart)
    {
        return new Cart_Model
        {
            CartId = cart.CartId,
            Car_Id = cart.Plate_Numbers,
            Total = cart.Total,
        };
    }
    public static IEntityBuilder<Cart, Cart_Model> FromModel(Cart_Model cart)
    {
        return Empty()
            .WithId(cart.CartId)
            .WithTotal(cart.Total);
    }

    public CartBuilder WithId(int cartId)
    {
        _cartId = cartId;
        return this;
    }
    public CartBuilder WithTotal(decimal total)
    {
        _total = total;
        return this;
    }

    public CartBuilder WithPlateNumbers(string plateNumbers)
    {
        _plateNumbers = plateNumbers;
        return this;
    }

    public static CartBuilder Empty()
    {
        return new CartBuilder();
    }

    public Cart Build()
    {
        return new Cart( _cartId, _plateNumbers, _total, null);
    }

    public static CartBuilder Modify(Cart cart)
    {
        return new CartBuilder()
            .WithId(cart.CartId)
            .WithTotal(cart.Total)
            .WithPlateNumbers(cart.Plate_Numbers);
    }

}
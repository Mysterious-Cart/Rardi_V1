namespace CHKS.Mappers;

using System.Linq.Expressions;
using Entity;
using Models;

public static class CartExpressionMapper
{
    public static Expression<Func<CartModel, Cart>> ToCart() =>
        model => new Cart(
            model.CartId,
            model.CustomerId,
            model.Total,
            model.CartContent.Select(model => new CartItem
        (
            model.ProductId,
            model.Qty,
            model.PriceOverwrite ?? model.Inventory.Export
        )).ToList()
        );

    public static Expression<Func<CartItemModel, CartItem>> ToCartItem() =>
        model => new CartItem
        (
            model.ProductId,
            model.Qty,
            model.PriceOverwrite ?? model.Inventory.Export
        );

}

public static class CartMapper
{
    public static Cart ToCart(this CartModel cartModel) =>
        new Cart
        (
            cartModel.CartId,
            cartModel.CustomerId,
            cartModel.Total,
            cartModel.CartContent.Select(CartExpressionMapper.ToCartItem().Compile()).ToList()
        );
}
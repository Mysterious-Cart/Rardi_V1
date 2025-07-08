namespace CHKS.Mappers;

using System.Linq.Expressions;
using CHKS.Models;
using CHKS.Entity;
using CHKS.Models.mydb;
using DocumentFormat.OpenXml.Office2010.Excel;

public class CartMapper
{
    public Expression<Func<CartModel, Cart>> ToCart() =>
        model => new Cart(
            model.CartId,
            model.Customer.Plate,
            model.Total,
            model.CartContent.Select(model => new CartItem
        (
            model.ProductId,
            model.Qty,
            model.PriceOverwrite ?? model.Inventory.Export
        )).ToList()
        );

    public Expression<Func<Entity.Cart, Models.CartModel>> ToCartModel()
    {
        return entity => new Models.CartModel
        {
            Id = entity.Id,
            Car_Id = entity.CarId,
            Customer = entity.Customer != null ? new Models.CustomerModel
            {
                Plate = entity.Customer.Plate,
                Name = entity.Customer.Name,
                Last_visit = entity.Customer.LastVisit,
                CreatedAt = entity.Customer.CreatedAt,
                Visits = entity.Customer.Visits,
                Total_visit = entity.Customer.TotalVisit
            } : null
        };
    }

    public Expression<Func<CartItemModel, CartItem>> ToCartItem() =>
        model => new CartItem
        (
            model.ProductId,
            model.Qty,
            model.PriceOverwrite??model.Inventory.Export
        );
}
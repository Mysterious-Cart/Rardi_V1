namespace CHKS.Mappers;

using Models;
using Entity;
using System.Linq.Expressions;
using System.Security.Claims;

public static class OrderMapper
{
    /// <summary>
    /// Converts an Order_Model to an Order.
    /// </summary>
    /// <param name="orderModel"></param>
    public static Order ToOrder(this OrderModel orderModel) =>
        new(
            orderModel.Id,
            orderModel.Amount,
            orderModel.OrderDate,
            orderModel.DeliveryDate,
            orderModel.OrderReceivedDate,
            orderModel.Description,
            orderModel.IsCancelled,
            orderModel.IsOrderReceived,
            orderModel.TotalPrice,
            orderModel.ProductId
        );
}

public static class OrderExpressionMapper
{
    /// <summary>
    /// Converts an Order_Model to an Order.
    /// This expression can be used in LINQ queries to project Order_Model to Order.
    /// </summary>
    public static Expression<Func<OrderModel, Order>> ToOrder(ClaimsPrincipal user) =>
        user.IsInRole("Admin") ?
        order => new Order(
            order.Id,
            order.Amount,
            order.OrderDate,
            order.DeliveryDate,
            order.OrderReceivedDate,
            order.Description,
            order.IsCancelled,
            order.IsOrderReceived,
            order.TotalPrice,
            order.ProductId
        ) :
        order => new Order(
            order.Id,
            order.Amount,
            order.OrderDate,
            order.DeliveryDate,
            order.OrderReceivedDate,
            order.Description,
            order.IsCancelled,
            order.IsOrderReceived,
            0.0m, // TotalPrice is not included for non-admin users
            order.ProductId
        );
}
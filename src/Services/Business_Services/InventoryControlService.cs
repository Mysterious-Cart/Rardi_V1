using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MoreLinq;
using FluentValidation;
using CHKS.Mappers;
using CHKS.Entity;
using CHKS.Models;
using CHKS.Validator;
using CHKS.Data;
using System.Data.Common;

namespace CHKS.Services;

//Exception note alway contain 'Cannot' for violation
/// <summary>
/// Service class for managing inventory control operations.
/// </summary>
public class InventoryControlService(
        ILogger<InventoryControlService> logger,
        IDbContextFactory<Rardi_Context> contextFactory,
        SecurityService securityService
    ) : IAsyncDisposable, IDisposable
{
    private readonly ILogger<InventoryControlService> logger = logger;
    private readonly Rardi_Context _context = contextFactory.CreateDbContext();
    private readonly SecurityService _securityService = securityService;
    
    #region Product Operations

    /// <summary>
    /// Retrieves a list of products.
    /// </summary>
    public async Task<List<Product>> GetProductList()
    {
        var product_source = _context.Inventory
            .AsNoTracking();
        var products = product_source.Select(ProductExpressionMapper.ToProduct);
        return await products.ToListAsync();
    }

    public async Task<List<Product>> GetProductList(string searchText)
    {
        var product_source = _context.Inventory
            .AsNoTracking()
            .Where(p => p.Name.Contains(searchText) || p.Description.Contains(searchText));
        var products = product_source.Select(ProductExpressionMapper.ToProduct);
        return await products.ToListAsync();
    }


    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="Id">The unique identifier of the product.</param>
    public async Task<Product> GetProductById(Guid Id)
    {
        return await _context.Inventory
            .AsNoTracking()
            .Select(ProductExpressionMapper.ToProduct)
            .FirstAsync(product => product.Id == Id);
        ;
    }

    public async Task<bool> IsStockAvailable(Guid Id, int Quantity)
    {

        var product = await _context.Inventory
            .AsNoTracking()
            .Select(product => new
            {
                product.Id,
                product.Stock
            })
            .FirstAsync(product => product.Id == Id);
        return product.Stock >= Quantity;
        //Check if stock is more or equal to the requested quantity
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when a duplicate product is detected.</exception>
    public async Task<Product> CreateProduct(CreateProductRequest product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");

        var validator = new ProductValidator();
        var validationResult = await validator.ValidateAsync(product);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product_model = product.ToProductModel();

        try
        {
            _context.Add(product_model);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to create product: {ProductName}", product.Name);
            throw new InvalidOperationException("Failed to create product.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating product: {ProductName}", product.Name);
            throw;
        }

        return product_model.ToProduct();
    }

    public async Task AddProductProfile()
    {

    }

    /// <summary>
    /// Adds items to the stock of a product.
    /// </summary>
    /// <param name="ProductId">The unique identifier of the product.</param>
    /// <param name="Add">The number of items to add.</param>
    public async Task AddItemToStock(Guid ProductId, int Add) => await ChangeStock(ProductId, Add);

    /// <summary>
    /// Removes items from the stock of a product.
    /// </summary>
    /// <param name="ProductId">The unique identifier of the product.</param>
    /// <param name="Deduct">The number of items to deduct.</param>
    public async Task RemoveItemFromStock(Guid ProductId, int Deduct) => await ChangeStock(ProductId, -Deduct);

    /// <exception cref="ArgumentException">Unexpected parameter</exception>
    /// <exception cref="InvalidOperationException">Operation Failed</exception>
    private async Task ChangeStock(Guid ProductId, int Changes)
    {
        if (ProductId == Guid.Empty) throw new ArgumentException("Product ID cannot be empty.", nameof(ProductId));
        if (Changes == 0) throw new ArgumentException("Changes cannot be zero.", nameof(Changes));

        if (Math.Abs(Changes) > 1000)
            throw new ArgumentException("Cannot deduct more than 1000 items at once.", nameof(Changes));

        var product = await _context.Inventory
            .AsNoTracking()
            .Select(i => new
            {
                i.Id,
                i.Name,
                i.Stock
            })
            .FirstAsync(i => i.Id == ProductId);

        if ((product.Stock - Changes) <= 0) throw new ArgumentException("Cannot exceed available stock.", nameof(Changes));

        try
        {
            await _context.Inventory.Where(i => i.Id == ProductId)
                .ExecuteUpdateAsync(i => i.SetProperty(x => x.Stock, x => x.Stock + Changes));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update stock for product: {ProductName}", product.Name);
            // Log the error and rethrow with a more specific message
            throw new InvalidOperationException($"Failed to update stock for product: {product.Name}", ex);
        }
    }
    
    #endregion

    #region Order Operations

    ///<summary>
    /// Creates an order for the specified product with the given amount.
    /// </summary>
    /// <param name="product">The product for which the order is to be created.</param>
    /// <param name="amount">The amount of the product to order. Must be greater than 0.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the product is not available to restock or the amount is less than 1.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown when the order creation fails.</exception>

    public async Task CreateOrder(Guid productId, int amount, string Description = "", DateOnly? deliveryDate = null)
    {
        if (amount < 1) throw new ArgumentException("Amount cannot be less then 1.", nameof(amount));
        if (productId == Guid.Empty) throw new ArgumentException("Product ID cannot be empty.", nameof(productId));

        var Product = await _context.Inventory
            .AsNoTracking()
            .Select(i => new
            {
                i.Id,
                i.AllowTracking
            })
            .FirstOrDefaultAsync(i => i.Id == productId);
        if (!Product.AllowTracking) throw new ArgumentException("This product is not available to restock.", nameof(productId));

        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            DeliveryDate = deliveryDate,
            OrderReceivedDate = null,
            Description = Description,
            IsCancelled = false,
            IsOrderReceived = false,
            ProductId = productId,
        };

        try
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        catch (DbException ex)
        {
            logger.LogError(ex, "Failed to create order for product {ProductId} with amount {Amount}.", productId, amount);
            throw new InvalidOperationException("Database error occurred while creating order.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while creating order for product {ProductId} with amount {Amount}.", productId, amount);
            throw;
        }

    }

    /// <summary>
    /// Confirms the order by setting the order as received and updating the stock.
    /// </summary>
    /// <param name="orderID">The unique identifier of the order to confirm.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <exception cref="ArgumentException">Thrown when the order ID is empty or the order does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the order confirmation fails.</exception>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ConfirmOrder(Guid orderID, CancellationToken token = default)
    {
        if (orderID == Guid.Empty) throw new ArgumentException("Order ID cannot be empty.", nameof(orderID));

        using var transaction = await _context.Database.BeginTransactionAsync(token);

        try
        {
            // Get order details first
            var order = await _context.Orders
                .AsNoTracking()
                .Select(i => new
                {
                    i.Id,
                    i.ProductId,
                    i.Amount,
                    i.IsOrderReceived,
                    i.OrderReceivedDate
                })
                .FirstAsync(i => i.Id == orderID, token);

            if (order.IsOrderReceived)
                throw new InvalidOperationException("Order has already been confirmed.");

            // Update the order status
            await _context.Orders
                .Where(i => i.Id == orderID)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(i => i.IsOrderReceived, true)
                    .SetProperty(i => i.OrderReceivedDate, DateOnly.FromDateTime(DateTime.Now)),
                    token);

            // Update the product stock
            await _context.Inventory
                .Where(i => i.Id == order.ProductId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(i => i.Stock, i => i.Stock + order.Amount),
                    token);

            await transaction.CommitAsync(token);

            logger.LogInformation("Order {OrderId} confirmed successfully", orderID);
        }
        catch (DbUpdateException dbEx)
        {

            logger.LogError(dbEx, "Database error occurred while confirming order {OrderId}", orderID);
            throw new InvalidOperationException($"Database error occurred while confirming order with ID: {orderID}", dbEx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to confirm order {OrderId}", orderID);
            throw new InvalidOperationException($"Failed to confirm order with ID: {orderID}", ex);
        }
    }

    /// <summary>
    /// Cancels the specified order.
    /// </summary>
    /// <param name="orderID">The unique identifier of the order to cancel.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the order ID is empty or the order does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the order is already cancelled or has been received.</exception>
    public async Task CancelOrder(Guid orderID)
    {
        if (orderID == Guid.Empty) throw new ArgumentException("Order ID cannot be empty.", nameof(orderID));
        try
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Select(i => new
                {
                    i.Id,
                    i.IsCancelled,
                    i.IsOrderReceived
                })
                .FirstAsync(i => i.Id == orderID);

            if (order.IsCancelled) throw new InvalidOperationException("Order is already cancelled.");

            if (order.IsOrderReceived) throw new InvalidOperationException("Cannot cancel order that has been received.");
            await _context.Orders.Where(i => i.Id == orderID)
                .ExecuteUpdateAsync(x => x.SetProperty(i => i.IsCancelled, true),
                CancellationToken.None);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Failed to cancel order with ID: {OrderId}", orderID);
            throw new InvalidOperationException($"Database error occurred while cancelling order with ID: {orderID}", dbEx);
        }
        catch (Exception exc)
        {
            Console.Write(exc.Message);
            throw new("Failed to cancel order.");
        }
    }

    /// <summary>
    /// Retrieves a list of product's orders.
    /// </summary>
    public async Task<List<Order>> GetOrders()
    {
        return await _context.Orders
            .AsNoTracking()
            .Select(OrderExpressionMapper.ToOrder(_securityService.Principal))
            .ToListAsync();
    }
    /// <summary>
    /// Retrieves a list of ongoing orders based on the search text.
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public async Task<List<Order>> GetOngoingOrders(string searchText = "")
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(i => i.Product)
            .Where(i => i.Product.Name.Contains(searchText))
            .OrderBy(i => i.IsCancelled)
            .ThenBy(i => i.IsOrderReceived)
            .ThenBy(i => i.OrderReceivedDate)
            .Select(OrderExpressionMapper.ToOrder(_securityService.Principal))
            .ToListAsync();
    }

    #endregion

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        // Release the DbContext if it implements IAsyncDisposable or IDisposable
        if (_context is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        // Release the DbContext if it implements IDisposable
        if (_context is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

}
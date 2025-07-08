using CHKS.Models.Interface;
using CHKS.Models.mydb;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MoreLinq;
using FluentValidation;
using CHKS.Mappers;
using CHKS.Models.Builder;
using CHKS.Entity;
using CHKS.Models;
using System;
using CHKS.Data;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace CHKS.Services;

//Exception note alway contain 'Cannot' for violation
/// <summary>
/// Service class for managing inventory control operations.
/// </summary>
public class InventoryControlService : IAsyncDisposable
{
    private readonly IDbProvider _provider;
    private readonly ILogger<InventoryControlService> logger;
    private readonly Rardi_Context _context;
    private readonly SecurityService _securityService;
    public InventoryControlService(
        IDbProvider provider,
        ILogger<InventoryControlService> logger,
        IDbContextFactory<Rardi_Context> contextFactory,
        SecurityService securityService
    )
    {
        _provider = provider;
        this.logger = logger;
        this._context = contextFactory.CreateDbContext();
        _securityService = securityService;
    }
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
    /*
    public record ProductIdentity(Guid Id, string Name);
    public async Task<Dictionary<Guid, double>> ClosestMatch(string Name, CancellationToken token = default, int Top = 50,
    IEnumerable<ProductIdentity> products = null, KhmerTokenizer tokenizer = null)
    {
        var result = new Dictionary<Guid, double>();
        var data = await GetProductList();
        var productIdentities = products is null ?
            data.Select(i => new ProductIdentity(i.Id, i.Name)).ToHashSet() :
            products.Select(i => new ProductIdentity(i.Id, i.Name)).ToHashSet();
        
        var ExactMatch = productIdentities.Where(i => i.Name.Contains(Name))??[];
        foreach (var match in ExactMatch)
        {
            result.Add(match.Id, 1.0);
        }
        productIdentities.RemoveWhere(i => ExactMatch.Contains(i));

        List<string> TokenizedName = [];
        bool useInternalTokenizer = tokenizer is null;
        tokenizer ??= new KhmerTokenizer();
        TokenizedName = tokenizer.Tokenize(Name, token);

        foreach (ProductIdentity identity in productIdentities)
        {
            if(token.IsCancellationRequested) break;

            var similarity = await tokenizer.CalculateTokenSimilarity(identity.Name, Name);
            if (similarity > 0)
            {
                result.Add(identity.Id, Math.Round(similarity,2));
            }
        }

        if(useInternalTokenizer)tokenizer.Dispose();

        return result.OrderByDescending(i => i.Value).Take(Top).ToDictionary();
        
    }

    public record InventoryIntegrityReport(double Duplicate, double Similar,double Ok ,int Total);

    // Not Working yet
    public async Task<InventoryIntegrityReport> GetInventoryIntegrityReport(CancellationToken token = default)
    {
        // RESULT: (Ok, 0.5) (Duplicate, 0.3) (Similar, 0.2)
        // Duplicate: When the highest similarity of the two product point to each other and above threshold.
        // EX: (ID: 355235, TO: 125325, Score: 0.8(Highest)) => (ID: 125325, TO: 355235, Score: 0.8(Also highest))
        // Similar: When the highest similarity of one product point to another product with lower similarity.
        // EX: (ID: 355235, TO: 125325, Score: 0.8(Highest)) => (ID: 125325, TO: 355235, Score: 0.8(but not the highest))

        const double Threshold = 0.65;

        int total_duplicate = 0;
        int total_similar = 0;

        var productsQuery = await GetProductList();

        Dictionary<Guid, Dictionary<Guid, double>> Similarity = [];
        using var tokenizer = new KhmerTokenizer();
        var products = productsQuery.Select(i => new ProductIdentity(i.Id, i.Name)).ToList();
        productsQuery = null;

        foreach (var product in products)
        {
            if (token.IsCancellationRequested) break;
            var result = await ClosestMatch(
                    product.Name, tokenizer: tokenizer, token: token, Top: 3, products: products);
            result.Remove(product.Id);
            Similarity.Add(product.Id, result);

        }

        tokenizer.Dispose();

        foreach (var product in Similarity)
        {
            if (token.IsCancellationRequested) break;

            var top_similarity_product = product.Value.First();

            if (Similarity.ContainsKey(top_similarity_product.Key) && Similarity[top_similarity_product.Key].Keys.First() == product.Key
                && top_similarity_product.Value > Threshold)
            {
                total_duplicate++;
            }
            else if (top_similarity_product.Value > Threshold)
            {
                total_similar++;
            }
        }

        if (token.IsCancellationRequested)
        {
            products.Clear();
            Similarity.Clear();
            return new InventoryIntegrityReport(0, 0, 0, 0);
        }
        ;

        double percentage_of_duplicate = (double)total_duplicate / products.Count * 100;
        double percentage_of_similar = (double)total_similar / products.Count * 100;

        return new InventoryIntegrityReport(percentage_of_duplicate, percentage_of_similar, 100 - (percentage_of_duplicate + percentage_of_similar), products.Count());
    }
    */
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
        if(productId == Guid.Empty) throw new ArgumentException("Product ID cannot be empty.", nameof(productId));

        var Product = await _context.Inventory
            .AsNoTracking()
            .Select(i => new
            {
                i.Id,
                i.AllowTracking
            })
            .FirstOrDefaultAsync(i => i.Id == productId);
        if (!Product.AllowTracking) throw new ArgumentException("This product is not available to restock.", nameof(productId));

        var order = new Order_Model
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
        catch
        {
            throw new InvalidOperationException("Failed to create order.");
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
                .FirstOrDefaultAsync(i => i.Id == orderID, token);

            if (order == null)
                throw new ArgumentException("Order does not exist.", nameof(orderID));

            if (order.IsOrderReceived)
                throw new InvalidOperationException("Order is already confirmed.");

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

            // Commit the transaction
            await transaction.CommitAsync(token);
        
            logger.LogInformation("Order {OrderId} confirmed successfully", orderID);
        }
        catch (Exception ex)
        {
            // Rollback is automatic when transaction is disposed without commit
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
        catch (Exception exc)
        {
            Console.Write(exc.Message);
            throw new("Failed to cancel order.");
        }
    }

    /// <summary>
    /// Retrieves a list of orders.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of orders.</returns>
    public async Task<IQueryable<Order>> GetOrders()
    {
        return _context.Orders
            .AsNoTracking()
            .Select(OrderExpressionMapper.ToOrder(_securityService.Principal));
    }

    #endregion

    #region  Tag Operations

    /// <summary>
    ///   a list of tags.   
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of tags.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the tag retrieval fails.</exception>
    public async Task<IEnumerable<Tag>> GetTags()
    {
        try
        {
            var tags = await _provider.GetData<Tags>();
            return tags.Select(i => Tag.FromTagModel(i));
        }
        catch (Exception exc)
        {
            logger.LogError(exc, exc.Message);
            throw new InvalidOperationException("Failed to retrieve tags.");
        }

    }


    /// <summary>
    /// Creates a new tag.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>

    public async Task CreateTag(Tag tag, bool Validation = true)
    {
        if (Validation) await ValidationCheck();

        try
        {
            await _provider.CreateData(new Models.mydb.Tags { Id = tag.Id, Color = "Yellow", Tag = tag.Name });
        }
        catch (Exception exc)
        {
            logger.LogError(exc, exc.Message);
            throw new InvalidOperationException("Failed to create tag.");
        }
        
        async Task ValidationCheck()
        {
            if (string.IsNullOrWhiteSpace(tag.Name)) throw new ArgumentException("Tag name cannot be empty.", nameof(tag));
            // Check for duplicate tag

            var tags = await _provider.GetData<Tags>();
            using var similarityChecker = new KhmerTokenizer();

            await foreach (var tagged in tags.AsAsyncEnumerable())
            {
                if (await similarityChecker.CalculateTokenSimilarity(tagged.Tag, tag.Name) > 0.6)
                {
                    similarityChecker.Dispose();
                    throw new ArgumentException("Cannot create duplicate tag. Similar Tag detected", nameof(tag));
                }
            }
                
        }
    }
    /// <summary>
    /// Updates the tag information.
    /// </summary>
    /// <param name="tag">The tag data transfer object containing the updated tag information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the tag ID is empty or the tag name is null, or when a similar tag already exists.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the tag update operation fails.
    /// </exception>
    public async Task UpdateTag(Tag tag)
    {
        if (tag.Id == Guid.Empty) throw new ArgumentException("Tag ID cannot be empty.", nameof(tag));
        var tags = await _provider.GetData<Tags>();
        if(tags.Any(i => i.Id.Equals(tag.Id))) throw new ArgumentException("Tag does not exist.", nameof(tag));
        if (tag.Name == null) throw new ArgumentException("Tag name cannot be empty.", nameof(tag));

        try
        {
            await _provider.UpdateData(new Tags { Id = tag.Id, Tag = tag.Name, Color = "Yellow" }, i => i.Id);
        }
        catch (Exception exc)
        {
            logger.LogError(exc, exc.Message);
            throw new InvalidOperationException("Failed to update tag.");
        }
    }
    #endregion

    
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        GC.Collect();
    }

}
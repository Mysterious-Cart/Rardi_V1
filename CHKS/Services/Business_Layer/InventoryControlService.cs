using CHKS.Models.Interface;
using CHKS.Models.mydb;
using Microsoft.EntityFrameworkCore;
using CHKS.Extras.Class.DTOs;
using System.Runtime.InteropServices;
using MoreLinq;
using CHKS.Models.Builder;
using CHKS.Entity;
namespace CHKS.Services;

//Exception note alway contain 'Cannot' for violation
/// <summary>
/// Service class for managing inventory control operations.
/// </summary>
public class InventoryControlService : IAsyncDisposable
{
    private readonly IDbProvider _provider;
    private readonly ILogger<InventoryControlService> logger;

    public InventoryControlService(IDbProvider provider, ILogger<InventoryControlService> logger)
    {
        _provider = provider;
        this.logger = logger;
    }
    #region Product Operations
    public async Task<IEnumerable<Product>> GetProductListDTO()
    {
        var products = await _provider.GetData<Product_Model>();
        return products.Select(product => ProductBuilder.FromModel(product).Build());
    }

    /// <summary>
    /// Retrieves a list of products.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of products.</returns>
    public async Task<IQueryable<Product>> GetProductList([Optional] string Name)
    {
        var products = await _provider.GetData<Product_Model>();
        if (Name is not null) products = products.Where(i => i.Name.Contains(Name));
        return from i in products orderby i.Stock descending select new Product
        (
            i.Id,
            i.Name,
            i.Stock,
            i.Import,
            i.Export,
            i.Status,
            i.AllowTracking,
            i.AllowWarning,
            i.Description
        );

    }
    public async Task<Product> GetProductDTOById(Guid Id)
    {
        var products = await _provider.GetData<Product_Model>();

        var product = await products
            .Select(product => ProductBuilder.FromModel(product).Build()).FirstAsync(i => i.Id == Id);
        return product;
    }

    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="Id">The unique identifier of the product.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the product.</returns>
    [Obsolete("Unnecessary method, use GetProductDTOById instead.")]
    public async Task<Product> GetProductById(Guid Id)
    {
        var products = await _provider.GetData<Product>();
        var product = await products.FirstAsync(i => i.Id == Id);
        return product;
    }

    public async Task UpdateProduct(Product product) => await _provider.UpdateData(ProductBuilder.ToModel(product), i => i.Id);
    
    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when a duplicate product is detected.</exception>
    public async Task CreateProduct(Product product)
    {
        var products = await GetProductList();
        var productNames = products.Select(i => i.Name);

        // Check for duplicate product (Required changes to be made)

        // List<string> TokenizedName = [];
        // using (var tokenizer = new KhmerTokenizer())
        // {
        //     TokenizedName = tokenizer.Tokenize(product.Name);

        // }

        //if (productNames.Any(i => i == product.Name)) throw new ArgumentException("Cannot create duplicate product.", nameof(product));

        await _provider.CreateData(ProductBuilder.ToModel(product));
    }

    /// <summary>
    /// Adds items to the stock of a product.
    /// </summary>
    /// <param name="ProductId">The unique identifier of the product.</param>
    /// <param name="Add">The number of items to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddItemToStock(Guid ProductId, int Add) => await ChangeStock(ProductId, Add);

    /// <summary>
    /// Removes items from the stock of a product.
    /// </summary>
    /// <param name="ProductId">The unique identifier of the product.</param>
    /// <param name="Deduct">The number of items to deduct.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RemoveItemFromStock(Guid ProductId, int Deduct) => await ChangeStock(ProductId, -Deduct);

    /// <exception cref="ArgumentException">Thrown when the resulting stock is less than zero.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the stock update fails.</exception>
    public async Task ChangeStock(Guid ProductId, int Changes)
    {
        var products = await _provider.GetData<Product_Model>();
        var product = await products.Select(i => new { i.Id, i.Stock}).FirstAsync(i => i.Id == ProductId);
        if (product.Stock < 0) throw new ArgumentException("Cannot exceed available stock.", nameof(Changes));

        try
        {
            await _provider.UpdateData<Product_Model, Guid>(product =>
                product.Stock += Changes
            , ProductId);
        }
        catch
        {
            throw new InvalidOperationException($"Failed to update stock for product with ID: {ProductId}");
        }
    }
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

            if (Similarity.ContainsKey(top_similarity_product.Key) && Similarity[top_similarity_product.Key].Keys.First() == product.Key && top_similarity_product.Value > Threshold)
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

    #endregion

    #region Order Operations

    ///<summary>
    /// Creates an order for the specified product with the given amount.
    /// </summary>
    /// <param name="product">The product for which the order is to be created.</param>
    /// <param name="amount">The amount of the product to order. Must be greater than 0.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the product is not available to restock or the amount is less than 1.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown when the order creation fails.</exception>

    public async Task CreateOrder(Product product, int amount)
    {
        if (!product.Setting.AllowTracking) throw new ArgumentException("This product is not available to restock.", nameof(product));
        if (amount < 1) throw new ArgumentException("Amount cannot be less then 1.", nameof(amount));

        var order = new Order
        {
            ProductId = product.Id,
            Amount = amount,
        };

        try
        {
            await _provider.CreateData(order);
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

        var orders = await _provider.GetData<Order>();
        var order = await orders.FirstAsync(i => i.Id == orderID, token) ??
            throw new ArgumentException("Order does not exist.", nameof(orderID));

        order.IsOrderReceived = true;
        order.OrderReceivedDate = DateOnly.FromDateTime(DateTime.Now);

        try
        {

            await _provider.Transaction(
                async () =>
                {
                    await AddItemToStock(order.ProductId, order.Amount);
                    await _provider.UpdateData(order, i => i.Id);
                }, token
            );

        }
        catch (Exception exc)
        {
            logger.LogError(exc, exc.Message);
            throw new InvalidOperationException($"Failed to confirm order with ID: {orderID}");
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
<<<<<<< Updated upstream
        if (orderID == Guid.Empty) throw new ArgumentException("Order ID cannot be empty.", nameof(orderID));

        var orders = await _provider.GetData<Order>();
        var order = await orders.FirstAsync(i => i.Id == orderID) ?? throw new ArgumentException("Order does not exist.", nameof(orderID));
        if (order.IsCancelled) throw new InvalidOperationException("Order is already cancelled.");

        if (order.IsOrderReceived) throw new InvalidOperationException("Cannot cancel order that has been received.");

        order.IsCancelled = true;
        try
        {
            await _provider.UpdateData(order, i => i.Id);
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
        return await _provider.GetData<Order>(["Product"]);
    }

    #endregion

    #region  Tag Operations

    /// <summary>
    /// Retrieves a list of tags.   
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of tags.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the tag retrieval fails.</exception>
    public async Task<IEnumerable<TagsDTO>> GetTags()
    {
        try
        {
            var tags = await _provider.GetData<Tags>();
            return tags.Select(i => TagsDTO.FromTags(i));
        }
        catch (Exception exc)
        {
            logger.LogError(exc, exc.Message);
            throw new InvalidOperationException("Failed to retrieve tags.");
        }

    }


=======
<<<<<<< Updated upstream
        var Inventory = await mydbService.GetInventories();
        Inventory = Inventory.Where(i => Tags.Any(v => i.Tags.All(z => z.Tag == v.Tag)));
        return Inventory;
    }


=======
        if (orderID == Guid.Empty) throw new ArgumentException("Order ID cannot be empty.", nameof(orderID));

        var orders = await _provider.GetData<Order>();
        var order = await orders.FirstAsync(i => i.Id == orderID, token) ??
            throw new ArgumentException("Order does not exist.", nameof(orderID));

        order.IsOrderReceived = true;
        order.OrderReceivedDate = DateOnly.FromDateTime(DateTime.Now);

        try
        {

            await _provider.Transaction(
                async () =>
                {
                    await AddItemToStock(order.ProductId, order.Amount);
                    await _provider.UpdateData(order, i => i.Id);
                }, token
            );

        }
        catch (Exception exc)
        {
            logger.LogError(exc, exc.Message);
            throw new InvalidOperationException($"Failed to confirm order with ID: {orderID}");
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

        var orders = await _provider.GetData<Order>();
        var order = await orders.FirstAsync(i => i.Id == orderID) ?? throw new ArgumentException("Order does not exist.", nameof(orderID));
        if (order.IsCancelled) throw new InvalidOperationException("Order is already cancelled.");

        if (order.IsOrderReceived) throw new InvalidOperationException("Cannot cancel order that has been received.");

        order.IsCancelled = true;
        try
        {
            await _provider.UpdateData(order, i => i.Id);
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
        return await _provider.GetData<Order>(["Product"]);
    }

    #endregion

    #region  Tag Operations

    /// <summary>
    /// Retrieves a list of tags.   
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


>>>>>>> Stashed changes
    /// <summary>
    /// Creates a new tag.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
<<<<<<< Updated upstream
    public async Task CreateTag(TagsDTO tag, bool Validation = true)
=======
    public async Task CreateTag(Tag tag, bool Validation = true)
>>>>>>> Stashed changes
    {
        if (Validation) await ValidationCheck();

        try
        {
<<<<<<< Updated upstream
            await _provider.CreateData(new Tags { Id = tag.Id, Color = "Yellow", Tag = tag.Name });
=======
            await _provider.CreateData(new Models.mydb.Tags { Id = tag.Id, Color = "Yellow", Tag = tag.Name });
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    public async Task UpdateTag(TagsDTO tag)
=======
    public async Task UpdateTag(Tag tag)
>>>>>>> Stashed changes
    {
        if (tag.Id == Guid.Empty) throw new ArgumentException("Tag ID cannot be empty.", nameof(tag));
        var tags = await _provider.GetData<Tags>();
        if(tags.Any(i => i.Id.Equals(tag.Id))) throw new ArgumentException("Tag does not exist.", nameof(tag));
        if (tag.Name == null) throw new ArgumentException("Tag name cannot be empty.", nameof(tag));

<<<<<<< Updated upstream
        using var tokenMatchEvaluator = new KhmerTokenizer();

        foreach (var tagged in tags.ToList())
        {
            if (await tokenMatchEvaluator.CalculateTokenSimilarity(tagged.Tag, tag.Name) > 0.6)
            {
                tokenMatchEvaluator.Dispose();
                throw new ArgumentException("Cannot create duplicate tag. Similar Tag detected", nameof(tag));
            }
        }

        tokenMatchEvaluator.Dispose();

=======
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    /// <summary>
    /// Tags the product by name using tokenized names and a similarity acceptance threshold.
    /// </summary>
    /// <param name="TokenizeName">The list of tokenized names to match against tags.</param>
    /// <param name="Acceptance">The similarity acceptance threshold. Default is 0.8.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of matched tags.</returns>
    public async Task<IEnumerable<TagsDTO>> TagProductByName(List<string> TokenizeName, double Acceptance = 0.6)
    {
        if(TokenizeName.Count == 0) throw new ArgumentException("Name cannot be empty.", nameof(TokenizeName));
        // Future changes: required multiple word to tag matching
        List<TagsDTO> TagMatched = [];
        var fetchedTags = await GetTags(); // Materialize the query results
        var NotCreatedTags = new List<TagsDTO>();
        using var matcher = new KhmerTokenizer();
        // Looping through all tokenize word
        foreach (string word in TokenizeName)
        {
            double MaximumSimilarity = 0.0;
            TagsDTO BestMatchingTag = null;
            // for each word, loop through all tags, and find the most similar tag
            foreach (TagsDTO tag in fetchedTags)
            {
                var tokenSimilarity = await matcher.CalculateTokenSimilarity(word, tag.Name);

                if (tokenSimilarity > Acceptance && tokenSimilarity > MaximumSimilarity)
                {
                    BestMatchingTag = tag;
                    MaximumSimilarity = tokenSimilarity;
                }
            }
            // Add the most similar tag to the list
            if (BestMatchingTag is not null) TagMatched.Add(BestMatchingTag);
            
        }

        matcher.Dispose();
        // Return the list of matched tags and remove duplicates
        return TagMatched?.DistinctBy(i => i.Name) ?? [];
    }

=======
>>>>>>> Stashed changes
    #endregion

    public async ValueTask DisposeAsync()
    {
        await _provider.DisposeAsync();
        GC.SuppressFinalize(this);
        GC.Collect();
    }
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
>>>>>>> Stashed changes


}
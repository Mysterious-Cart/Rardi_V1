using Microsoft.EntityFrameworkCore;

namespace CHKS.Services;

using Entity;
using Data;
using Models;
using Mappers;
using System.Data.Common;

public class CartControlService(
    InventoryControlService stockcontrol,
    IDbContextFactory<Rardi_Context> contextFactory,
    SecurityService securityService,
    ILogger<CartControlService> logger
    )
{
    private readonly ILogger<CartControlService> _logger = logger;
    private readonly InventoryControlService stockControl = stockcontrol;
    private readonly Rardi_Context _context = contextFactory.CreateDbContext();
    private readonly SecurityService _securityService = securityService;

    /// <summary>
    /// Processes the checkout for a cart with the specified CartId.
    /// This method creates a transaction record and associated transaction items based on the cart contents,
    /// saves them to the database within a transaction scope, and commits the transaction if successful.
    /// If any database update error occurs, the transaction is rolled back and an exception is thrown.
    /// </summary>
    /// <param name="CartId">The unique identifier of the cart to be checked out.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the cart checkout fails due to a database error or other failure during transaction creation.
    /// </exception>
    public async Task ProcessCartCheckout(int CartId)
    {
        var user = _securityService.User?.Id ?? throw new UnauthorizedAccessException("User not Found.");
        var transactionEntity = await _context.Carts
            .Where(i => i.CartId == CartId)
            .Select(Cart => new TransactionModel
            {
                Plate = Cart.CustomerId,
                EmployeeId = user,
            }
            ).FirstAsync();

        var transaction = await _context.Database.BeginTransactionAsync();
        {
            try
            {
                // Add and save the transaction entity first to generate its Id
                await _context.AddAsync(transactionEntity);
                await _context.SaveChangesAsync();

                // Now fetch cart items and assign the generated Id
                List<TransactionItemModel> transactionItems =
                    await _context.CartContents.Where(i => i.CartId == CartId)
                        .Select(item => new TransactionItemModel
                        {
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            Price = item.PriceOverwrite ?? item.Inventory.Export,
                            TransactionId = transactionEntity.Id
                        }).ToListAsync();

                await _context.AddRangeAsync(transactionItems);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Database update error: {dbEx}");
                throw new InvalidOperationException("Failed to process cart checkout due to database error.", dbEx);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Failed to create transaction.");
            }
        }

    }
    
    /// <summary>
    /// creates a new cart for the specified customer.
    /// </summary>
    /// <param name="customer">The customer for whom the cart is to be created.</param>
    /// <returns>
    /// A <see cref="Cart"/> object representing the newly created cart.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the cart could not be created due to a database error or other unexpected exception.
    /// </exception>
    public async Task<Cart> AddCart(Customer customer)
    {
        var cart = new CartModel
        {
            CustomerId = customer.Plate,
        };

        try
        {
            await _context.AddAsync(cart);
            await _context.SaveChangesAsync();
            return CartMapper.ToCart(cart);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Failed to create cart for customer {CustomerPlate}", customer.Plate);
            throw new InvalidOperationException("Failed to create cart due to database error.", dbEx);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to create cart for customer {CustomerPlate}", customer.Plate);
            throw;
        }
    }

    /// <summary>
    /// Adds a product to the specified cart.
    /// This method checks if the product is already in the cart and updates the quantity if it is.
    /// If the product is not in the cart, it adds a new cart item.
    /// </summary>
    /// <param name="CartId"></param>
    /// <param name="productId"></param>
    /// <param name="Qty"></param>
    /// <param name="price"></param>
    /// <param name="Note"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Cart> AddProductToCart(int CartId, Guid productId, int Qty = 1, decimal? price = null, string Note = "")
    {
        var cartItem = new CartItemModel
        {
            CartId = CartId,
            ProductId = productId,
            Qty = Qty,
            PriceOverwrite = price,
            Note = Note
        };

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Qty, nameof(Qty)); // Ensure Qty is positive
        if (!await stockControl.IsStockAvailable(productId, Qty)) throw new ArgumentException("Product not available"); // Check stock availability

        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var cartcontent = _context.CartContents.Where(i => i.ProductId == productId && i.CartId == CartId);
            if (await cartcontent.AnyAsync())
            {
                // Product already exists in the cart, update quantity
                await stockControl.RemoveItemFromStock(productId, Qty);
                await cartcontent.ExecuteUpdateAsync(v => v.SetProperty(i => i.Qty, i => i.Qty + Qty));
            }
            else
            {
                // Product does not exist in the cart, add new item
                await stockControl.RemoveItemFromStock(productId, Qty);
                await _context.AddAsync(cartItem);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return CartMapper.ToCart(cartItem.Cart);
        }
        catch (Exception exc)
        {
            await _context.Database.RollbackTransactionAsync();
            _logger.LogError(exc, "Failed to add product {ProductId} to cart {CartId}", productId, CartId);
            Console.WriteLine(exc.Message);
            throw;
        }

    }

    /// <summary>
    /// Removes a product from the specified cart.
    /// If the quantity to remove is greater than or equal to the existing quantity, the item is removed entirely.
    /// If the quantity to remove is less than the existing quantity, the quantity is reduced accordingly
    /// </summary>
    /// <param name="CartId"></param>
    /// <param name="productId"></param>
    /// <param name="Deduction"></param>
    /// <returns></returns>
    public async Task<Cart> RemoveProductFromCart(int CartId, Guid productId, int Deduction = 1)
    {

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Deduction, nameof(Deduction)); // Ensure Qty is positive

        try
        {
            var cartcontents = _context.CartContents
                .Where(i => i.CartId == CartId && i.ProductId == productId);

            using var transaction = await _context.Database.BeginTransactionAsync();

            var cartcontent = await cartcontents.FirstAsync();
            var AmountInCart = cartcontent.Qty;
            if (AmountInCart <= Deduction)
            {
                // If the quantity to remove is greater than or equal to the existing quantity, remove the item
                await stockControl.AddItemToStock(productId, AmountInCart);
                await _context.CartContents.Where(i => i.CartId == CartId && i.ProductId == productId).ExecuteDeleteAsync();
            }
            else
            {
                await stockControl.AddItemToStock(productId, Deduction);
                await cartcontents.ExecuteUpdateAsync(v => v.SetProperty(i => i.Qty, i => i.Qty - Deduction));
            }

            await transaction.CommitAsync();
            return await GetCart(CartId);

        }
        catch (ArgumentNullException exc)
        {
            _logger.LogError(exc, "Failed to remove product {ProductId} from cart {CartId}. Likely due to Product doesn't exist.", productId, CartId);
            Console.WriteLine(exc.Message);
        }

        return null;
    }

    /// <summary>
    /// Retrieves all carts from the database.
    /// This method includes related customer and cart content information.
    /// </summary>
    /// <returns></returns>
    public async Task<List<Cart>> GetCart()
    {
        var cart = await _context.Carts
            .Include(i => i.CartContent)
            .ThenInclude(i => i.Inventory)
            .Select(CartExpressionMapper.ToCart())
            .ToListAsync();

        return cart;
    }

    public async Task<Cart> GetCart(int CartId)
    {
        var cart = await _context.Carts
            .Include(i => i.CartContent)
            .ThenInclude(i => i.Inventory)
            .Where(i => i.CartId == CartId)
            .Select(CartExpressionMapper.ToCart())
            .FirstAsync();

        return cart;
    }

    /// <summary>
    /// Adds a new customer to the database.
    /// This method checks if a customer with the same plate number already exists
    /// </summary>
    /// <param name="customer"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task AddCustomer(CreateCustomerRequest customer)
    {
        string plate = customer.Plate.Replace(" ", "").ToUpper();
        var existed = await _context.Customers.AnyAsync(i => i.PlateNumber == plate);
        if (existed)
        {
            throw new ArgumentException("Customer already exists.");
        }

        try
        {
            var model = new CustomerModel
            {
                PlateNumber = plate,
                Name = customer.Name,
                Phone = customer.Phone,
                Phone_2 = customer.Phone2,
                Description = customer.Description,
            };

            await _context.Customers.AddAsync(model);
            await _context.SaveChangesAsync();
        }
        catch (DbException dbEx)
        {
            Console.WriteLine($"Database error: {dbEx.Message}");
            _logger.LogError(dbEx, "Failed to create customer with plate {Plate}", plate);
            throw new InvalidOperationException("Failed to create customer due to database error.", dbEx);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            _logger.LogError(exc, "Failed to create customer with plate {Plate}", plate);
            throw new InvalidOperationException("Failed to create customer.");
        }

    }
    public async Task<List<Customer>> GetCustomer()
    {
        return _context.Customers
            .Select(CustomerExpressionMapper.ToCustomer())
            .ToList();
    }

    public async Task<Customer> GetCustomer(string Plate)
    {
        return await _context.Customers
            .Select(CustomerExpressionMapper.ToCustomer())
            .FirstAsync(i => i.Plate == Plate);
    }
    
}
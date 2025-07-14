using Microsoft.EntityFrameworkCore;
using CHKS.Entity;
using CHKS.Data;
using CHKS.Models;
using CHKS.Mappers;

namespace CHKS.Services;

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
    /*
    public async Task UpdateCartItem(int CartId, Product product, decimal? price = null, string Note = "") => await UpdateCartItem(CartId, new CartItemDTO
    {
        Id = Guid.NewGuid(),
        CartId = CartId,
        ProductId = product.Id,
        Quantity = 1,
        Original_Price = product.Export,
        Set_Price = price,
        Note = Note,
    });
    public async Task UpdateCartItem(int CartId, CartItemDTO cartItem, CancellationToken token = default)
    {
        var CartItemList = await GetCartContent(CartId);
        var CartItem_Found = await CartItemList.FirstAsync(i => i.ProductId == cartItem.Id, token);
        var changes_amount = cartItem.Quantity - CartItem_Found.Quantity;

        CartItem newCartItem = new()
        {
            CartId = CartId,
            ProductId = cartItem.ProductId,
            Qty = cartItem.Quantity,
            PriceOverwrite = cartItem.Set_Price,
            Note = cartItem.Note,
        };

        Task operation = CartItem_Found is null ?
            _provider.CreateData(newCartItem) :
            cartItem.Quantity == 0 ?
                RemoveCartItem(cartItem, token) :
                _provider.UpdateData(cartItem, i => i.Id);

        try
        {
            await _provider.Transaction(async () =>
            {
                await stockControl.ChangeStock(cartItem.ProductId, changes_amount);
                await operation;
            }, token);

        }
        catch (Exception exc)
        {
            Console.WriteLine("Error detected: " + exc.Message);
            //Cancel Operation and Revert Operations
            // Finish Later.
        }
    }

    public async Task RemoveCartItem(CartItemDTO CartItem, CancellationToken token = default)
    {
        try
        {
            await _provider.Transaction(async () =>
            {
                await _provider.DeleteData<CartItem, Guid>(i => i.Id, CartItem.Id);
                await stockControl.AddItemToStock(CartItem.ProductId, CartItem.Quantity);
            }, token);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
        }
    }

    public async Task RemoveCart(int CartId)
    {
        var CartList = await _provider.GetData<Cart>();
        var Cart = await CartList.FirstAsync(i => i.CartId == CartId);
        try
        {
            await _provider.DeleteData<Cart, int>(i => i.CartId, Cart.CartId);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
        }
    }
*/

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
            var transaction = await _context.Database.BeginTransactionAsync();
            if (await _context.CartContents.AnyAsync(i => i.ProductId == productId && i.CartId == CartId))
            {

                // Product already exists in the cart, update quantity
                await stockControl.RemoveItemFromStock(productId, Qty);
                await _context.CartContents
                    .Where(i => i.CartId == CartId && i.ProductId == productId)
                    .ExecuteUpdateAsync(v => v.SetProperty(i => i.Qty, i => i.Qty + Qty));
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
            Console.WriteLine(exc.Message);
        }
        
        return null; // Return null if an error occurs
    }

    public async Task<bool> RemoveProductFromCart(int CartId, Guid productId, int Qty = 1)
    {

        try
        {
            if(!(await GetCart()).ToList().Any(i => i.CartId == CartId))
            {
                return false; // Cart does not exist
            }

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Qty, nameof(Qty)); // Ensure Qty is positive
            var cartContents = await _provider.GetData<CartItemModel>();
            var content = cartContents.Where(i => i.CartId == CartId).ToList();

            if (cartContents.Any(i => i.ProductId == productId))
            {
                // Product already exists in the cart, update quantity
                var existingItem = content.First(i => i.CartId == CartId && i.ProductId == productId);
                if (existingItem.Qty - Qty <= 0)
                {
                    // Product exists in the cart, remove it
                    await stockControl.AddItemToStock(productId, existingItem.Qty);
                    await _provider.DeleteData<CartItemModel, int>(CartId);
                    return true;
                }

                existingItem.Qty--;
                await stockControl.ChangeStock(productId, 1);
                await _provider.UpdateData(existingItem, i => i.Id);
                return true;
            }
            
            return false;
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            return false;
        }
    }


    public async Task<IEnumerable<Cart>> GetCart()
    {
        var cart = _context.Carts
            .Include(i => i.Customer)
            .Include(i => i.CartContent)
            .Select(CartExpressionMapper.ToCart());

        return cart;
    }

    public async Task<Cart> GetCart(int CartId)
    {
        var cart = await _context.Carts
            .Include(i => i.Customer)
            .Include(i => i.CartContent)
            .Where(i => i.CartId == CartId)
            .Select(CartExpressionMapper.ToCart())
            .FirstAsync();

        return cart;
    }

    /* 
    public async Task<Cart> GetCartContent(int CartId)
    {
        var Items = await _provider.GetData<CartModel>([nameof(CartModel.CartContent)]);
        Items.Include(i => i.CartContent.Select(i => i.Inventory));
        var result =
            from i in Items
            where i.CartId == CartId
            select new Cart(
                i.CartId,
                i.Customer.Plate,
                i.Total,
                from j in i.CartContent
                select new CartItem(j.ProductId, j.Inventory.Name, j.Qty, j.Inventory.Import,
                j.PriceOverwrite ?? j.Inventory.Export)
            );

        return await result.FirstAsync();
    }*/

    public async Task AddCustomer(Customer customer)
    {
        var customers = await _provider.GetData<Models.mydb.CustomerModel>();
        string plate = customer.Plate.Replace(" ", "").ToUpper();
        if (customers.Any(i => i.Plate == plate))
        {
            throw new ArgumentException("Customer already exists.");
        }

        try
        {
            var model = CustomerBuilder.ToModel(customer);
            model.Last_visit = DateOnly.FromDateTime(DateTime.Now);
            model.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            model.Visits = 1;

            await _provider.CreateData(model);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            throw new InvalidOperationException("Failed to create customer.");
        }
    }
    public async Task<IEnumerable<Entity.Customer>> GetCustomer()
    {
        var customers = await _provider.GetData<Models.mydb.CustomerModel>();
        customers.Include(i => i.Vehicle);
        
        return from i in customers
               select new Entity.Customer(
            i.Plate,
            i.Name,
            i.Phone,
            new Vehicle(i.Vehicle_Id, i.Vehicle.Model, i.Vehicle.Make, i.Vehicle.Year),
            i.Phone_2,
            i.Description   
        );
    }

    public async Task<Entity.Customer> GetCustomer(string Plate)
    {
        var customers = await _context.Customers
            .Select(CustomerExpressionMapper.ToCustomer())
            .ToListAsync();

        var customer = await customers.FirstOrDefaultAsync(i => i.Plate == Plate);

        if (customer is null) return null;

        return CustomerBuilder.FromModel(customer).Build();
    }
    public async Task<IEnumerable<Vehicle>> GetVehicles()
    {
        var Vehicle = await _provider.GetData<Vehicle_Model>();

        return from i in Vehicle orderby i.Year descending select new Vehicle(i.Key, i.Model, i.Make, i.Year) ;
    }
    public async Task AddVehicle(Vehicle vehicle)
    {
        var Vehicle = await _provider.GetData<Vehicle_Model>();

        if (await Vehicle.Where(i => i.Key == vehicle.GetId()).AnyAsync())
        {
            throw new ArgumentException("Vehicle already exists.");
        }
        try
        {
            await _provider.CreateData(VehicleBuilder.ToModel(vehicle));
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
        }
    }
}
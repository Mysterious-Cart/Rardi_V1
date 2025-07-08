using CHKS.Models.mydb;
using Microsoft.EntityFrameworkCore;
using CHKS.Entity;
using CHKS.Models.Interface;

namespace CHKS.Services;

public class CartControlService(InventoryControlService stockcontrol, IDbProvider provider)
{
    private readonly InventoryControlService stockControl = stockcontrol;

    private readonly IDbProvider _provider = provider;
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
    public async Task Cashout(int CartId)
    {
        var CartList = await _provider.GetData<CartModel>();
        var Cart = await CartList.Include(i => i.CartContent).FirstAsync(i => i.CartId == CartId);

        Models.mydb.TransactionModel transaction = new()
        {
            Plate = Cart.Car_Id,
            Total = Cart.Total,
        };

        try
        {
            await _provider.CreateData(transaction);
        }
        catch
        {
            throw new InvalidOperationException("Failed to create transaction.");
        }

    }



    public async Task<Cart> AddCart(Entity.Customer customer)
    {
        var cart =
            
        var cart_Model = CartBuilder.ToModel(cart);

        try
        {
            await _provider.CreateData(cart_Model);
            return cart;
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            throw new Exception("Failed to create cart.");
        }
    }
    public async Task<bool> AddProductToCart(int CartId, Guid productId,int Qty = 1 ,decimal? price = null, string Note = "")
    {
        var cartItem = new CartItemModel
        {
            CartId = CartId,
            ProductId = productId,
            Qty = Qty,
            PriceOverwrite = price,
            Note = Note
        };

        try
        {
            if(!(await GetCart()).Any(i => i.CartId == CartId))
            {
                return false; // Cart does not exist
            }
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Qty, nameof(Qty)); // Ensure Qty is positive
            if (!await stockControl.IsStockAvailable(productId, Qty)) return false;

            var cartContents = await _provider.GetData<CartItemModel>();
            var content = cartContents.Where(i => i.CartId == CartId);
            if (content.Any(i => i.ProductId == productId))
            {
                // Product already exists in the cart, update quantity
                var existingItem = content.First(i => i.CartId == CartId && i.ProductId == productId);
                existingItem.Qty++;
                await stockControl.RemoveItemFromStock(productId, Qty);
                await _provider.UpdateData(existingItem, i => i.Id);
                return true;
            }
            
            await stockControl.RemoveItemFromStock(productId, Qty);
            await _provider.CreateData(cartItem);
            // Product does not exist in the cart, add it
            return true;
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            return false;
        }
    }

    public async Task<bool> RemoveProductFromCart(int CartId, Guid productId, int Qty = 1, decimal? price = null, string Note = "")
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
        var cart = await _provider.GetData<CartModel>();

        return from i in cart
               select new Cart(
                i.CartId,
                i.Customer.Plate,
                i.Total,
                null
        );
    }

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
    }

    public async Task AddCustomer(Entity.Customer customer)
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
        var customers = await _provider.GetData<Models.mydb.CustomerModel>();

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
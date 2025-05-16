using CHKS.Models.mydb;
using Microsoft.EntityFrameworkCore;
using CHKS.Entity;
using CHKS.Models.Interface;
using CHKS.Extras.Class.DTOs;
using CHKS.Entity;
using DocumentFormat.OpenXml.Office.CustomUI;

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

    public async Task Cashout(int CartId)
    {
        var CartList = await _provider.GetData<Cart>();
        var Cart = await CartList.Include(i => i.CartContent).FirstAsync(i => i.CartId == CartId);

        History transaction = new()
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

    */

     public async Task<Cart> AddCart(Customer customer)
    {
        var cart =
            CartBuilder.Empty()
                    .WithId(Random.Shared.Next(1, 1000))
                    .WithTotal(0)
                    .WithPlateNumbers(customer.Plate)
                    .Build();
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

    public async Task<IEnumerable<Cart>> GetCart()
    {
        var cart = await _provider.GetData<Cart_Model>();

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
        var Items = await _provider.GetData<Cart_Model>([nameof(Cart_Model.CartContent), nameof(CartItem_Model.Inventory)]);

        var result =
            from i in Items
            where i.CartId == CartId
            select new Cart(
                i.CartId,
                i.Customer.Plate,
                i.Total,
                from j in i.CartContent
                    select new CartItem(j.ProductId, j.Inventory.Name, j.Qty, j.PriceOverwrite ?? j.Inventory.Export, j.Total)
            );

        return await result.FirstAsync();
    }

    public async Task AddCustomer(Customer customer)
    {
        var customers = await _provider.GetData<Customer_Model>();
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
            model.Total_visit = 1;

            await _provider.CreateData(model);
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
            throw new InvalidOperationException("Failed to create customer.");
        }
    }
    public async Task<IEnumerable<Customer>> GetCustomer()
    {
        var customers = await _provider.GetData<Customer_Model>();
        customers.Include(i => i.Vehicle);
        
        return from i in customers
               select new Customer(
            i.Plate,
            i.Name,
            i.Phone,
            new Vehicle(i.Vehicle_Id, i.Vehicle.Model, i.Vehicle.Make, i.Vehicle.Year),
            i.Phone_2,
            i.Description   
        );
    }

    public async Task<Customer> GetCustomer(string Plate)
    {
        var customers = await _provider.GetData<Customer_Model>();

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
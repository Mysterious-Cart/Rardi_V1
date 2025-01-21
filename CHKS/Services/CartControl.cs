using CHKS.Models.mydb;
using CHKS.Models;
using Radzen;
using Microsoft.EntityFrameworkCore;

namespace CHKS;

public class CartControlService
{
    private readonly mydbService mydbService;
    private readonly StockControlService stockControl;

    private readonly IDbProvider _provider;

    public CartControlService(mydbService MydbService, StockControlService stockcontrol, IDbProvider provider)
    {
        this.mydbService = MydbService;
        this.stockControl = stockcontrol;
        this._provider = provider;
    }

    public async Task<bool> AddItemToCart(int CartId, Inventory Item, int Qty = 1, string Note = "", decimal? priceOverride = null){
        var CartItemList = await GetCartItemFromCart(CartId);
        var CartItems = CartItemList.Where(i => i.ProductId == Item.Id);
        try{
            await stockControl.RemoveItemToStock(Item.Id, Qty);
            if(CartItems.Any()){
                var CartItem = CartItems.First();
                CartItem.Qty += Qty;
                
                if(CartItem.Qty <= 0){
                    await mydbService.DeleteConnector(CartItem.Id);
                }else{
                    await mydbService.UpdateConnector(CartItem.Id,CartItem);
                }
            }else{
                Connector newCartItem = new(){
                    CartId = CartId,
                    ProductId = Item.Id,
                    Qty = Qty,
                    PriceOverwrite = priceOverride,
                    Note = Note,
                };
                await mydbService.CreateConnector(newCartItem);
            }
            
            return true;
        }catch(Exception exc){
            Console.WriteLine("Error detected: " + exc.Message);
            return false;
            //Cancel Operation and Revert Operations
            // Finish Later.
        }
    }

    public async Task<bool> RemoveCartItem(Connector CartItem){
        try{
            await mydbService.DeleteConnector(CartItem.Id);
            await stockControl.AddItemToStock(CartItem.ProductId, (int)CartItem.Qty);
            return true;
        }catch(Exception exc){
            Console.WriteLine(exc.Message);
            return false;
        }
    }

    public async Task<IQueryable<Connector>> GetCartItemFromCart(int CartId){
        var Items = await _provider.GetData<Connector>([nameof(Connector.Inventory)]);
        
        return Items.Where(i => i.CartId == CartId);
    }
}
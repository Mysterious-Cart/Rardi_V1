using CHKS.Models;
using CHKS.Models.mydb;
using Microsoft.AspNetCore.Components;

namespace CHKS;

public partial class StockControlService
{

    private readonly mydbService mydbService;

    private readonly IDbProvider provider;

    public StockControlService(mydbService MydbService, IDbProvider provider)
    {
        this.mydbService = MydbService;
        this.provider = provider;
    }

    public async Task NewOrder(Guid ProductId, int IncomingAmount)
    {

    }

    partial void OnProductDetailChanged(Inventory product);
    public async Task AddItemToStock(Guid ProductId, int Add){
        var data = await provider.GetData<Inventory>();
        var product = data.First(i => i.Id == ProductId);
        product.Stock += Add;
        try{
            await provider.UpdateData<Inventory, Guid>(product, i => i.Id);
            OnProductDetailChanged(product);
        }catch{
            throw new Exception("Failed to Add to stock.");
        }
    }

    public async Task RemoveItemToStock(Guid ProductId, int Deduct){
        Inventory product = await mydbService.GetInventoryById(ProductId);
        if(product.Stock < Deduct){
            throw new Exception("Not enough item in stock.");
        }
        product.Stock -= Deduct;
        
        try{
            await mydbService.UpdateInventory(product);
            OnProductDetailChanged(product);
        }catch{
            throw new Exception("Failed to Remove from stock.");
        }
    }

    public async Task<IEnumerable<Inventory>> FilterByTag(List<Tags> Tags){
        var Inventory = await mydbService.GetInventories();
        Inventory = Inventory.Where(i => Tags.Any(v => i.Tags.All(z => z.Tag == v.Tag)));
        return Inventory;
    }


    

}
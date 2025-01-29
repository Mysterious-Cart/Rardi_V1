using CHKS.Models.Interface;
using CHKS.Models.mydb;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Services;

public partial class StockControlService
{

    private readonly mydbService mydbService;
    private readonly IDbProvider _provider;

    public StockControlService(mydbService MydbService, IDbProvider provider)
    {
        mydbService = MydbService;
        _provider = provider;
    }

    partial void OnProductDetailChanged(Inventory product);

    public async Task AddItemToStock(Guid ProductId, int Add)
    {
        var ProductList = await _provider.GetData<Inventory>();
        Inventory product = await ProductList.FirstAsync(i => i.Id == ProductId);
        product.Stock += Add;
        try
        {
            OnProductDetailChanged(product);
        }
        catch
        {
            throw new Exception("Failed to Add to stock.");
        }
    }

    public async Task RemoveItemFromStock(Guid ProductId, int Deduct)
    {
        Inventory product = await mydbService.GetInventoryById(ProductId);
        if (product.Stock < Deduct)
        {
            throw new Exception("Not enough item in stock.");
        }
        product.Stock -= Deduct;

        try
        {
            await mydbService.UpdateInventory(product);
            OnProductDetailChanged(product);
        }
        catch
        {
            throw new Exception("Failed to Remove from stock.");
        }
    }

    public async Task<IEnumerable<Inventory>> FilterByTag(List<Tags> Tags)
    {
        var Inventory = await mydbService.GetInventories();
        Inventory = Inventory.Where(i => Tags.Any(v => i.Tags.All(z => z.Tag == v.Tag)));
        return Inventory;
    }




}
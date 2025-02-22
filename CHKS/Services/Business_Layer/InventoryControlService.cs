using CHKS.Models.Interface;
using CHKS.Models.mydb;
using CHKS.Models.Class;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Services;

public partial class InventoryControlService
{

    private readonly mydbService mydbService;
    private readonly IDbProvider _provider;

    public InventoryControlService(mydbService MydbService, IDbProvider provider)
    {
        mydbService = MydbService;
        _provider = provider;
    }

    public async Task<List<ProductDTO>> GetProductList(){
        var products =  await _provider.GetData<Inventory>();
        return await products.Select(i => i.ToDTO()).ToListAsync();
    }

    public async Task CreateProduct(Inventory product){
        await _provider.CreateData(product);
    }

    
    public async Task AddItemToStock(Guid ProductId, int Add)
    {
        var ProductList = await _provider.GetData<Inventory>();
        var product = await ProductList
                .Select(i => new{i.Id, i.Stock})
                .FirstAsync(i => i.Id == ProductId);

        int newStock = (int)product.Stock + Add;
        try
        {
            await _provider.UpdateData(product,i => i.Id);
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
            throw new ArgumentOutOfRangeException("Not enough item in stock.");
        }
        product.Stock -= Deduct;

        try
        {
            await _provider.UpdateData(product, i => i.Id);
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
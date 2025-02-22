using CHKS.Models.Class;
using CHKS.Models.mydb;
using CHKS.Models.Interface;
using MudBlazor;
using Microsoft.EntityFrameworkCore;
namespace CHKS;

public static class DTOMapper
{
    public static ProductDTO ToDTO(this Inventory product){
        return new ProductDTO(
            product.Id, 
            product.Name,
            (int)product.Stock, 
            product.Import, 
            product.Export);
    }
    
    public static async Task<Inventory> LoadSource(this ProductDTO productDTO, IDbProvider provider){
        var data = await provider.GetData<Inventory>();
        return await data.FirstAsync(i => i.Id == productDTO.Id);
    }
}

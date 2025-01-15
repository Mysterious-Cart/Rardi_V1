using CHKS.Models.mydb;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CHKS.Models.mydb;

[PrimaryKey("Id")]
public class Tags : ITableFormat<Tags>, IItem
{

    [Key]
    [Required]
    public Guid Id {get; set;} = Guid.NewGuid();

    public string Tag {get; set;}

    public string Color {get; set;}

    public ICollection<Inventory> Product;

    public static async Task<Tags> Create(mydbService service , Tags Item){
        return await service.CreateTag(Item);
    }
    public static async Task<bool> Remove(mydbService service, Guid ItemId){
        try{
            var result = await service.DeleteTags(ItemId);
            return result is not null? true: false;
        } catch(Exception exc){
            return false;
        }
    }

    public static async Task<Tags> Update(mydbService service, Tags Item){
        return await service.UpdateTags(Item);
    }

}
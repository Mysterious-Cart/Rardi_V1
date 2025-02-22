using CHKS.Models.mydb;
using System.ComponentModel.DataAnnotations;

namespace CHKS.Models.Class;
public class ProductDTO{

    [Key]
    public readonly Guid Id;
    public readonly string Name;
    public readonly int Qty;
    public readonly decimal Import;
    public readonly decimal Export;

    public ProductDTO(Guid Id, string Name, int Qty, decimal Import, decimal Export){
        this.Id = Id;
        this.Name = Name;
        this. Qty = Qty;
        this.Import = Import;
        this.Export = Export;
    }

}
using CHKS.Models.Enum;
using CHKS.Extras.Class.DTOs;

namespace CHKS.Entity;
public class Product
{
    public readonly Guid Id;
    public readonly string Name;
    public readonly int Stock;
    public readonly decimal Import;
    public readonly decimal Export;
    public readonly string Description;
    public readonly ProductStatus Status;
    public ProductSetting Setting;
    public List<TagsDTO> Tags { get; set; } = [];
    private bool AllowTracking{ get; set; }
    private bool AllowWarning { get; set; }

    public Product(Guid Id, string Name, int Stock,
        decimal Import,
        decimal Export,
        ProductStatus Status,
        bool AllowTracking,
        bool AllowWarning,
        string Description = ""
        )
    {
        this.Id = Id;
        this.Name = Name;
        this.Stock = Stock;
        this.Import = Import;
        this.Export = Export;
        this.Status = Status;
        this.Description = Description;
        this.AllowTracking = AllowTracking;
        this.AllowWarning = AllowWarning;

        this.Setting = new ProductSetting(AllowTracking, AllowWarning);

    }

}
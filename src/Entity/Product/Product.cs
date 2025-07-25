namespace CHKS.Entity;
using Enum;

public record Product(Guid Id, string Name, int Stock, decimal Import, decimal Export, string Description, ProductStatus Status, ProductSetting Setting)
{
    public override string ToString()
    {
        return $"{Name} - {Description} - {Stock} in stock - {Export:C}";
    }
}
public record CreateProductRequest(
    string Name,
    int Stock,
    decimal Import,
    decimal Export,
    string Description,
    ProductStatus Status,
    ProductSetting Setting
);

public record ProductSetting(
    bool IsTracking,
    bool IsAllowLowWarning,
    List<Tag> Tags = null);
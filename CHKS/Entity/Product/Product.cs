using CHKS.Enum;


namespace CHKS.Entity;

public record Product(Guid Id, string Name, int Stock, decimal Export, string Description, ProductStatus Status)
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
    ProductStatus Status)
{
    public override string ToString()
    {
        return $"{Name} - {Description} - {Stock} in stock - {Export:C}";
    }
}


    

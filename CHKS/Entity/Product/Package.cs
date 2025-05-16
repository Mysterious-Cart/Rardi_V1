using CHKS.Entity;

public class Package
{
    public Guid Id { get; set; }
    public string Description { get; set; } = "";
    public IEnumerable<Product> Products { get; set; } = [];
}
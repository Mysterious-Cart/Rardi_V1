using System.Runtime.InteropServices;
using CHKS.Entity;

namespace CHKS.Entity;
public abstract class CartBase<ChildItem>(string Plate_Numbers, decimal total, [Optional] IEnumerable<ChildItem> items)
{
    public decimal Total { get; set; } = total;
    public string Plate_Numbers { get; set; } = Plate_Numbers;
    public IEnumerable<ChildItem> Items { get; set; } = items ?? [];
}
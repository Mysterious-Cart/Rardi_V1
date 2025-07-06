using CHKS.Models.mydb;

namespace CHKS.Models
{
    public partial class Product_Model
    {
        public ICollection<CartItem_Model> Connectors { get; set; }
        public ICollection<Historyconnector> HistoryConnectors { get; set; }
        public ICollection<Tags> Tags { get; set; }
        public ICollection<Order_Model> Orders { get; set; }
        public ICollection<StockLogs> StockLogs { get; set; }
        public ICollection<ProductProfiles> ProductProfiles { get; set; }
    }
}

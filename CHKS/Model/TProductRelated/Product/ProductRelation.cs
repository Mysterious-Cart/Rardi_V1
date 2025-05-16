using CHKS.Models.mydb;

namespace CHKS.Models.mydb
{
    public partial class Product_Model
    {
        public ICollection<CartItem_Model> Connectors { get; set; }
        public ICollection<Historyconnector> HistoryConnectors { get; set; }
        public ICollection<Tags> Tags { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}

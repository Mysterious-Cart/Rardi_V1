using CHKS.Models.mydb;

namespace CHKS.Models
{
    public partial class Product_Model
    {
        public ICollection<CartItemModel> CartItems { get; set; }
        public ICollection<TransactionItemModel> TransactionItems { get; set; }
        public ICollection<Tags> Tags { get; set; }
        public ICollection<OrderModel> Orders { get; set; }
        public ICollection<StockLogs> StockLogs { get; set; }
        public ICollection<ProductProfiles> ProductProfiles { get; set; }
    }
}

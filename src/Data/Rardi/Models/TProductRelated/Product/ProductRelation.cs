using CHKS.Models.mydb;

namespace CHKS.Models
{
    public partial class ProductModel
    {
        public ICollection<CartItemModel> CartItems { get; set; }
        public ICollection<TransactionItemModel> TransactionItems { get; set; }
        public ICollection<TagsModel> Tags { get; set; }
        public ICollection<OrderModel> Orders { get; set; }
        public ICollection<StockLogsModel> StockLogs { get; set; }
        public ICollection<ProductProfiles> ProductProfiles { get; set; }
    }
}

using CHKS.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CHKS.Models
{
    [Table("Cart")]
    public class CartModel
    {
        [Key]
        [Column("CartID")]
        public int CartId { get; set; } = Random.Shared.Next();

        [Column("CarID")]
        [Required]
        [MaxLength(20)]
        public string CustomerId { get; set; } 

        public CustomerModel Customer { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0;

        [Required]
        public CartStatus Status { get; set; } = CartStatus.Progress;

        public ICollection<CartItemModel> CartContent { get; set; }

    }
}
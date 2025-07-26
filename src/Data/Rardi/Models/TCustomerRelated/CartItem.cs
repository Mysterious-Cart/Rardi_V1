using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models
{
    [Table("CartItem")]
    [PrimaryKey("Id")]
    public class CartItemModel
    {
        [Required]
        public int CartId { get; set; }
        public CartModel Cart { get; }

        [Required]
        public Guid ProductId { get; set; }

        public ProductModel Inventory { get; }

        [Required]
        public int Qty { get; set; }
        
        public string Note { get; set; } = "";

        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        public decimal? PriceOverwrite { get; set; } = null;
    }
}
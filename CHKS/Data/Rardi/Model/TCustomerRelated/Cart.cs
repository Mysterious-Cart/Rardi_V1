using CHKS.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CHKS.Models
{
    [Table("cart")]
    public class CartModel
    {
        [Key]
        [Column("CartID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; } // Remove random generation

        [Column("CarID")]
        [Required]
        [MaxLength(20)]
        public string Car_Id { get; set; }

        public CustomerModel Customer { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0;

        [Required]
        public short Status { get; set; } = 0;

        // Use ICollection for navigation properties
        public ICollection<CartItemModel> CartContent { get; set; } = new List<CartItemModel>();

        // Computed properties
        [NotMapped]
        public int ItemCount => CartContent?.Count ?? 0;

    }
}
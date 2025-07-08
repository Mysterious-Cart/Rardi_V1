using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CHKS.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models
{
    [Table("connector")]
    [PrimaryKey("Id")]
    public class CartItemModel
    {
        [Required]
        public int CartId { get; set; }

        public CartModel Cart { get; }

        [Required]
        public Guid ProductId { get; set; }

        public Product_Model Inventory { get; }

        [Required]
        public int Qty { get; set; }

        public string Note { get; set; } = "";

        [Key]
        [Required]
        public Guid Id { get; } = Guid.NewGuid();

        public decimal? PriceOverwrite { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CHKS.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models.mydb
{
    [Table("connector")]
    [PrimaryKey("Id")]
    public class CartItem_Model
    {
        [Required]
        public int CartId { get; set; }

        public Cart_Model Cart { get; }

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
        public decimal Total => PriceOverwrite ?? Inventory.Export * Qty;

    }
}
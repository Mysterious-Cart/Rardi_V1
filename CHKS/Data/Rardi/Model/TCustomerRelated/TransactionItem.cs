using CHKS.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models
{
    [Table("historyconnector")]
    public class TransactionItemModel
    {
        public decimal Qty { get; set; }

        public decimal Export { get; set; }

        [Column("CartID")]
        [Required]
        public Guid CartId { get; set; }

        public TransactionModel Transaction { get; set; }

        public string Remark { get; set; } = "";

        [Key]
        [Column("ID")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("ProductId")]
        [Required]
        public Guid ProductId { get; set; }

        public Product_Model Inventory {get; set;}
    }
}
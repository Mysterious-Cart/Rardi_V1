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
        public int Qty { get; set; }

        public decimal Price { get; set; }

        [Column("CartID")]
        [Required]
        [ForeignKey("Transaction")]
        public Guid TransactionId { get; set; }
        public TransactionModel Transaction { get; set; }

        public string Remark { get; set; } = "";

        [Key]
        [Column("ID")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("ProductId")]
        [Required]
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product_Model Product {get; set;}
    }
}
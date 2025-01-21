using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("historyconnector")]
    public class Historyconnector : IModelClass
    {
        public decimal Qty { get; set; }

        public decimal Export { get; set; }

        [Column("CartID")]
        [Required]
        public Guid CartId { get; set; }

        public History History { get; set; }

        public string Note { get; set; } = "";

        [Key]
        [Column("ID")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("ProductId")]
        [Required]
        public Guid ProductId { get; set; }

        public Inventory Inventory {get; set;}
    }
}
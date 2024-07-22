using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("connector")]
    public partial class Connector
    {
        [Required]
        public int CartId { get; set; }

        [Required]
        public string Product { get; set; }

        public Inventory Inventory { get; set; }

        [Required]
        public decimal Qty { get; set; }

        [Required]
        public string Note { get; set; }

        [Key]
        [Required]
        public string GeneratedKey { get; set; }

        public decimal? PriceOverwrite { get; set; }

        public decimal? Discount { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.fordev
{
    [Table("connector")]
    public partial class Connector
    {
        [Required]
        public int CartId { get; set; }

        public Cart Cart { get; set; }

        [Required]
        public string Product { get; set; }

        public Inventory Inventory { get; set; }

        [Required]
        public decimal Qty { get; set; }

        public string Note { get; set; }

        [Key]
        [Required]
        public string GeneratedKey { get; set; }

        [Required]
        public decimal Total { get; set; }
    }
}
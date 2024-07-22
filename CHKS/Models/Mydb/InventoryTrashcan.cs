using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("inventorytrashcan")]
    public partial class Inventorytrashcan
    {
        [Key]
        [Required]
        public string Date { get; set; }

        public string Name { get; set; }

        public decimal? Import { get; set; }

        public decimal? Export { get; set; }

        public decimal? Stock { get; set; }

        public long? Barcode { get; set; }
    }
}
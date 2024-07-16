using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("inventory_trashcan")]
    public partial class InventoryTrashcan
    {
        [Key]
        [Column("date")]
        [Required]
        public string Date { get; set; }

        public string Name { get; set; }

        public decimal? Stock { get; set; }

        public decimal? Import { get; set; }

        public decimal? Export { get; set; }

        public string Barcode { get; set; }

        [Column("inventory_trashcancol")]
        public string InventoryTrashcancol { get; set; }
    }
}
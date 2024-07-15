using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("inventory")]
    public partial class Inventory
    {
        [Key]
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Stock { get; set; }

        public decimal? Import { get; set; }

        public decimal? Export { get; set; }

        public long? Barcode { get; set; }

        [Column("Normalize_Name")]
        public string NormalizeName { get; set; }

        public ICollection<Connector> Connectors { get; set; }

        public ICollection<Historyconnector> Historyconnectors { get; set; }
    }
}
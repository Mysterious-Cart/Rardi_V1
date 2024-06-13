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
        public int Stock { get; set; }

        public float? Import { get; set; }

        public float? Export { get; set; }

        public ICollection<Connector> Connectors { get; set; }

        public ICollection<Historyconnector> Historyconnectors { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("inventory_productgroup")]
    public partial class InventoryProductgroup
    {
        public string GroupName { get; set; }

        [Key]
        [Required]
        public string Key { get; set; }
    }
}
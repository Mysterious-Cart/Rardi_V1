using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("inventory_option")]
    public partial class InventoryOption
    {
        [Key]
        [Required]
        public string Key { get; set; }

        public string GroupName { get; set; }

        public string Option { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("inventory_caroption")]
    public partial class InventoryCaroption
    {
        [Key]
        [Required]
        public string Key { get; set; }

        public string Option { get; set; }

        public string Car { get; set; }
    }
}
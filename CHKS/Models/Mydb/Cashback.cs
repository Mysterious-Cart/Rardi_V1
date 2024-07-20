using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("cashback")]
    public partial class Cashback
    {
        [Key]
        [Required]
        public string Key { get; set; }

        public string Name { get; set; }

        public decimal? Amount { get; set; }
    }
}
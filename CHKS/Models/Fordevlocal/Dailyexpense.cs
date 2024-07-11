using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.fordevlocal
{
    [Table("dailyexpense")]
    public partial class Dailyexpense
    {
        [Required]
        public string Note { get; set; }

        public decimal? Expense { get; set; }

        [Key]
        [Required]
        public string Key { get; set; }
    }
}
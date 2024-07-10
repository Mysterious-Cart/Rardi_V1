using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.fordev
{
    [Table("dailyexpense")]
    public partial class Dailyexpense
    {
        [Key]
        [Required]
        public string Note { get; set; }

        public decimal? Expense { get; set; }
    }
}
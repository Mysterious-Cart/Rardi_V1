using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("dailyexpense")]
    public partial class Dailyexpense
    {
        public string Note { get; set; }

        public decimal? Expense { get; set; }

        [Key]
        [Required]
        public string Date { get; set; }
    }
}
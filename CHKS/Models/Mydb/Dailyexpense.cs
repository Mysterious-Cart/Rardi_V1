using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("dailyexpense")]
    public partial class Dailyexpense
    {
        [Key]
        [Required]
        public string Note { get; set; }

        public float? Expense { get; set; }
    }
}
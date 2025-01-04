using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("dailyexpense")]
    public partial class Dailyexpense
    {
        [Required]
        public string Note { get; set; } = "";

        public decimal Expense { get; set; } = 0;

        [Key]
        [Required]
        public Guid Key { get; set; } = Guid.NewGuid();

        public string Date {get; set;}

    }
}
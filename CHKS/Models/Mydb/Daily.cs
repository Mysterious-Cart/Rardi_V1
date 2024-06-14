using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("daily")]
    public partial class Daily
    {
        [Key]
        [Required]
        public int Date { get; set; }

        public decimal? Total { get; set; }

        public decimal? Expense { get; set; }

        public decimal? ProductExpense { get; set; }
    }
}
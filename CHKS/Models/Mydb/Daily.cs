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

        public float? Total { get; set; }

        public float? Expense { get; set; }

        public float? ProductExpense { get; set; }
    }
}
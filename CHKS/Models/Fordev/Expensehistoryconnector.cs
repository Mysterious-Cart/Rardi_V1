using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.fordev
{
    [Table("expensehistoryconnector")]
    public partial class Expensehistoryconnector
    {
        [Key]
        [Required]
        public string Date { get; set; }

        public decimal? Total { get; set; }

        public string Note { get; set; }
    }
}
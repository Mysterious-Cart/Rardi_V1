using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("expensehistoryconnector")]
    public partial class Expensehistoryconnector
    {
        [Key]
        [Column("history")]
        [Required]
        public string History { get; set; }

        public History History1 { get; set; }

        public decimal? Total { get; set; }

        public string Note { get; set; }
    }
}
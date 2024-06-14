using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("history")]
    public partial class History
    {
        [Key]
        [Required]
        public string CashoutDate { get; set; }

        [Required]
        public string Plate { get; set; }

        public Car Car { get; set; }

        public decimal? Total { get; set; }

        public string Payment { get; set; }

        public ICollection<Historyconnector> Historyconnectors { get; set; }
    }
}
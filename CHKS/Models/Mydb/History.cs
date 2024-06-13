using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("history")]
    public partial class History
    {
        [Required]
        public string CashoutDate { get; set; }

        [Required]
        public string Plate { get; set; }

        public Car Car { get; set; }

        public float? Total { get; set; }

        public string Payment { get; set; }

        [Key]
        [Column("HistoryConnector_ID")]
        [Required]
        public string HistoryConnectorId { get; set; }

        public ICollection<Historyconnector> Historyconnectors { get; set; }
    }
}
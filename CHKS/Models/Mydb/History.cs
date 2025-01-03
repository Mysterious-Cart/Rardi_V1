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

        public decimal? Bank { get; set; }

        public decimal? Dollar { get; set; }

        public decimal? Baht { get; set; }

        public decimal? Riel { get; set; }

        public sbyte? Company { get; set; }

        public string User { get; set; }

        public string Info { get; set; }

        [Required]
        public short IsDeleted { get; set; }

        public ICollection<Historyconnector> Historyconnectors { get; set; }
    }
}
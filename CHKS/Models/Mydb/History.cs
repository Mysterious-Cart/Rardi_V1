using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("history")]
    public class History : IModelClass
    {
        [Required]
        public string CashoutDate { get; set;} = DateTime.Now.ToString("dd/MM/yyyy");

        [Required]
        public string Plate { get; set; }

        public Car Car { get; set; }

        public decimal? Total { get; set;} = 0;

        public decimal? Bank { get; set; } = 0;

        public decimal? Dollar { get; set; } = 0;

        public decimal? Baht { get; set; } = 0;

        public decimal? Riel { get; set; } = 0;

        public sbyte? Company { get; set; } = 0;

        public string User { get; set; } = "";

        public string Info { get; set; } = "";

        public short Status {get; set;} = 0;

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public IEnumerable<Historyconnector> Historyconnectors { get; set; }
    }
}
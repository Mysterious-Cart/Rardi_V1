using CHKS.Models.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models
{
    [Table("history")]
    public class TransactionModel
    {
        [Required]
        public string CashoutDate { get; set;} = DateTime.Now.ToString("dd/MM/yyyy");

        [Required]
        public string Plate { get; set; }

        public CustomerModel Customer { get; set; }

        public decimal? Total { get; set;} = 0;

        public decimal? Bank { get; set; } = 0;

        public decimal? Dollar { get; set; } = 0;

        public decimal? Baht { get; set; } = 0;

        public decimal? Riel { get; set; } = 0;
        public string User { get; set; } = "";

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public ICollection<TransactionItemModel> TransactionItems { get; set; }
    }
}
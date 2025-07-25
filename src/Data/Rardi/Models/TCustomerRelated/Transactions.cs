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
        public readonly string CashoutDate = DateTime.Now.ToString("dd/MM/yyyy");

        [Required]
        public string Plate { get; set; }

        public CustomerModel Customer { get; set; }
        
        [Required]
        [ForeignKey("User")]
        public string EmployeeId { get; set; }
        public ApplicationUser User { get; set; }

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public ICollection<TransactionItemModel> TransactionItems { get; set; }
        public ICollection<PaymentModel> Payments { get; set; }
    }
}
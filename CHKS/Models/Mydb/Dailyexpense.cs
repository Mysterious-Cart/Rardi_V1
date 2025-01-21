using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("dailyexpense")]
    public class Dailyexpense : IModelClass
    {
        [Required]
        public string Note { get; set; } = "";

        public decimal Expense { get; set; } = 0;

        [Key]
        [Required]
        public Guid Key { get; set; } = Guid.NewGuid();

        [Required]
        public string Date {get; set;} = DateTime.Now.ToString("dd/MM/yyyy");

    }
}
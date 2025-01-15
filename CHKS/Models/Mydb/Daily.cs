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

        public decimal Total { get; set; }

        public decimal Expense { get; set; }

        public decimal Import_Total { get; set; }

        public int Total_Product_Sold {get; set;}

        public int Total_Cart {get; set;}
        
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CHKS.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models.mydb
{
    [Table("connector")]
    [PrimaryKey("Id")]
    public class Connector : IItem, IModelClass
    {
        [Required]
        public int CartId { get; set; }

        public Cart Cart { get; }

        [Required]
        public Guid ProductId { get; set; }

        public Inventory Inventory {get;}

        [Required]
        public decimal Qty { get; set; }

        public string Note { get; set; } = "";

        [NotMapped]
        public decimal Export {get; set;}

        [Key]
        [Required]
        public Guid Id { get; } = Guid.NewGuid();

        public decimal? PriceOverwrite { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("historyconnector")]
    public partial class Historyconnector
    {
        [Key]
        [Column("ID")]
        [Required]
        public string Id { get; set; }

        public string Product { get; set; }

        public int? Qty { get; set; }

        public decimal? Export { get; set; }

        public decimal? Import { get; set; }

        [Column("CartID")]
        [Required]
        public string CartId { get; set; }

        public History History { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("cart")]
    public partial class Cart
    {
        [Required]
        public string Plate { get; set; }

        public Car Car { get; set; }

        [Key]
        [Column("CartID")]
        [Required]
        public int CartId { get; set; }

        public decimal? Total { get; set; }

        public string Creator { get; set; }

        public short? Company { get; set; }

        public ICollection<Connector> Connectors { get; set; }
    }
}
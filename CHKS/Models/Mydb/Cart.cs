using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("cart")]
    public partial class Cart
    {
        [Column("CarID")]
        [Required]
        public string Car_Id { get; set; }

        public Car Car { get; set; }

        [Key]
        [Column("CartID")]
        [Required]
        public int CartId { get; set; } = new Random().Next(0, 1000);

        [Required]
        public decimal Total { get; set; } = 0;

        [Required]
        public short Status {get; set;} = 0;

        public ICollection<Connector> Connectors { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("car")]
    public partial class Car
    {
        [Key]
        [Required]
        public string Plate { get; set; }

        public string Phone { get; set; }

        public string Type { get; set; }

        public string Info { get; set; }

        public short? IsDeleted { get; set; }

        public string Key { get; set; }

        public ICollection<Cart> Carts { get; set; }

        public ICollection<History> Histories { get; set; }
    }
}
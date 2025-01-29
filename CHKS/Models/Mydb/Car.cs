using CHKS.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("car")]
    public class Car : IModelClass
    {
        [Key]
        [Required]
        public string Plate { get; set; }

        public string Phone { get; set; } = "";

        public string Type { get; set; } = "";

        public short IsDeleted { get; set; } = 0;

        public string Info { get; set; } = "";

        public ICollection<Cart> Carts { get; set; }

        public ICollection<History> Histories { get; set; }

        public Car()
        {
            Plate = Plate?.Trim().ToUpper();
        }
    }
}
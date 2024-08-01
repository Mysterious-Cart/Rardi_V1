using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("car_brand")]
    public partial class CarBrand
    {
        [Required]
        public string Brand { get; set; }

        public string Car { get; set; }

        [Key]
        [Required]
        public string Key { get; set; }
    }
}
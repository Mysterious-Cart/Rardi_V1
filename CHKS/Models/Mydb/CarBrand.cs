using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("car_brand")]
    public class CarBrand
    {
        [Required]
        public string Brand { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Year {get; set;}

        [Key]
        [Required]
        public string Key { get; set; }
    }
}
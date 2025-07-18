using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models
{
    [Table("vehicle")]
    public class Vehicle_Model
    {
        [Required]
        public string Model { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public int Year { get; set; }

        [Key]
        [Required]
        public int Key { get; set; }

        public ICollection<CustomerModel> Customer { get; set; }
        
    }
}
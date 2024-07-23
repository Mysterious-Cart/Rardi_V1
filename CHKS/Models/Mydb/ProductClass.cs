using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("product_classes")]
    public partial class ProductClass
    {
        [Key]
        [Column("ID")]
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
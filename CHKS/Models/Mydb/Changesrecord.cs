using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("changesrecord")]
    public partial class Changesrecord
    {
        [Key]
        [Required]
        public string DateOfChange { get; set; }

        public string Info { get; set; }

        public string User { get; set; }
    }
}
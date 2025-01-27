using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("changesrecord")]
    public partial class Records
    {
        [Key]
        [Required]
        public string Date { get; set; }

        public string Info { get; set; }

        public string User { get; set; }
    }
}
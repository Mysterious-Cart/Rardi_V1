using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("__efmigrationshistory")]
    public partial class Efmigrationshistory
    {
        [Key]
        [Required]
        public string MigrationId { get; set; }

        [Required]
        public string ProductVersion { get; set; }
    }
}
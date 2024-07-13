using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.fordevlocal
{
    [Table("aspnetuserroles")]
    public partial class Aspnetuserrole
    {
        [Key]
        [Required]
        public string UserId { get; set; }

        public Aspnetuser Aspnetuser { get; set; }

        [Key]
        [Required]
        public string RoleId { get; set; }

        public Aspnetrole Aspnetrole { get; set; }
    }
}
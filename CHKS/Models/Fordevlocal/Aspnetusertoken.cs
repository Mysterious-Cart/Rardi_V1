using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.fordevlocal
{
    [Table("aspnetusertokens")]
    public partial class Aspnetusertoken
    {
        [Key]
        [Required]
        public string UserId { get; set; }

        public Aspnetuser Aspnetuser { get; set; }

        [Key]
        [Required]
        public string LoginProvider { get; set; }

        [Key]
        [Required]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
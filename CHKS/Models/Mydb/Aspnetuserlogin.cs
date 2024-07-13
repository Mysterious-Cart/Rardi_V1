using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("aspnetuserlogins")]
    public partial class Aspnetuserlogin
    {
        [Key]
        [Required]
        public string LoginProvider { get; set; }

        [Key]
        [Required]
        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }

        [Required]
        public string UserId { get; set; }

        public Aspnetuser Aspnetuser { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("aspnetroleclaims")]
    public partial class Aspnetroleclaim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        [Required]
        public string RoleId { get; set; }

        public Aspnetrole Aspnetrole { get; set; }
    }
}
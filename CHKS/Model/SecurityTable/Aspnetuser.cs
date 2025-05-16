using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("aspnetusers")]
    public partial class Aspnetuser
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public int AccessFailedCount { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public string NormalizedEmail { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string SecurityStamp { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string UserName { get; set; }

        public ICollection<Aspnetuserclaim> Aspnetuserclaims { get; set; }

        public ICollection<Aspnetuserlogin> Aspnetuserlogins { get; set; }

        public ICollection<Aspnetuserrole> Aspnetuserroles { get; set; }

        public ICollection<Aspnetusertoken> Aspnetusertokens { get; set; }
    }
}
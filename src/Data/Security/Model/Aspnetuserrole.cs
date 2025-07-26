using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace CHKS.Models
{
    [Table("AspNetUserRoles")]
    [PrimaryKey(nameof(UserId), nameof(RoleId))]
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
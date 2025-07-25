using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models
{
    [Table("Groups")]	
    public class GroupModel
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<EmployeeModel> Employee { get; set; }
    }
}
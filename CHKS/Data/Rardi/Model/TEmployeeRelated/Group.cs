using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models
{
    [Table("Groups")]	
    public class Groups
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<Employee> Employee { get; set; } = new List<Employee>();
    }
}
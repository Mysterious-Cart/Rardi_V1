using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("dailyexpense")]
    public class Dailyexpense
    {
        [Required]
        public string Note { get; set; } = "";
        
        [DefaultValue(0)]
        public decimal Expense { get; set; } = 0;

        [Key]
        [Required]
        public Guid Key { get; init; } = Guid.NewGuid();

        [Required]
        public DateOnly Date {get; set;} = DateOnly.FromDateTime(DateTime.Now);

    }
}
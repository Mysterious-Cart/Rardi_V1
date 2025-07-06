using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models.mydb
{
    [Table("customer")]
    public class Customer
    {
        [Key]
        [Required]
        public string Plate { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; } = "";
        public string? Phone_2 { get; set; }
        public string? Description { get; set; }
        public int Total_visit { get; set; } = 0;

        [Column(TypeName = "date")]
        public DateOnly? Last_visit { get; set; }
        
        [Column(TypeName = "date")]
        public DateOnly? CreatedAt { get; set; }
        [Required]
        public int Vehicle_Id { get; set; }
        public Vehicle_Model Vehicle { get; set; }

        public ICollection<Cart_Model> Carts { get; set; }
        public ICollection<History> Histories { get; set; }

    }
}
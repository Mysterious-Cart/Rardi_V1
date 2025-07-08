using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models
{
    [Table("customer")]
    public class CustomerModel
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
        public int Visits { get; set; } = 0;

        [Column(TypeName = "date")]
        public DateOnly? Last_visit { get; set; }
        
        [Column(TypeName = "date")]
        public DateOnly? CreatedAt { get; set; }
        [Required]
        public int Vehicle_Id { get; set; }
        public Vehicle_Model Vehicle { get; set; }

        public ICollection<CartModel> Carts { get; set; }
        public ICollection<TransactionModel> Transactions { get; set; }

    }
}
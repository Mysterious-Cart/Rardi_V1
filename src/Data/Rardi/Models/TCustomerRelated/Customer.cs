using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CHKS.Models
{
    [Table("customer")]
    public class CustomerModel
    {
        [Key]
        [Required]
        public string PlateNumber { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; } = string.Empty;
        [AllowNull]
        [MaxLength(15)]
        [Column("phone_2", TypeName = "varchar(15)")]
        public string Phone_2 { get; set; }
        [AllowNull]
        [MaxLength(500)]
        [Column(TypeName = "text")]
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? Last_visit { get; set; }
        
        [Column(TypeName = "date")]
        public DateOnly CreatedAt => DateOnly.FromDateTime(DateTime.Now);

        [Required]
        public int Vehicle_Id { get; set; }
        public Vehicle_Model Vehicle { get; set; }
        public ICollection<CartModel> Carts { get; set; }
        public ICollection<TransactionModel> Transactions { get; set; }

    }
}
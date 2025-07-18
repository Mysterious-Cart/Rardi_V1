using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CHKS.Enum;

namespace CHKS.Models
{
    [Table("inventory")]
    public partial class Product_Model
    {

        [Required]
        public int Stock { get; set; } = 0;
        [Required]
        public decimal Export { get; set; } = 0;
        [Required]
        public string Barcode { get; set; } = "";
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Normalized_Name { get; set; } = "";
        
        [Required]
        public ProductStatus Status { get; set; } = ProductStatus.Active;

        [Required]
        public string Description { get; set; } = "";

        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();
        
    }
}
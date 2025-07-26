using System.ComponentModel.DataAnnotations;

namespace CHKS.Models
{
    public partial class ProductModel
    {
        [Required]
        public decimal Import { get; set; } = 0;
    }
}

using System.ComponentModel.DataAnnotations;

namespace CHKS.Models.mydb
{
    public partial class Product_Model
    {
        [Required]
        public decimal Import { get; set; } = 0;
    }
}

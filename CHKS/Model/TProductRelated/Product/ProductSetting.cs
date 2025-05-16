using System.ComponentModel.DataAnnotations;

namespace CHKS.Models.mydb
{
    public partial class Product_Model
    {
        [Required]
        public bool AllowTracking { get; set; } = true;

        [Required]
        public bool AllowWarning { get; set; } = true;

        [Required]
        public int Optimal_Stock { get; set; } = 0;

    }
}

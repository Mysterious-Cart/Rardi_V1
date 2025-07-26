using System.ComponentModel.DataAnnotations;

namespace CHKS.Models
{
    public partial class ProductModel
    {
        [Required]
        public bool AllowTracking { get; set; } = true;

        [Required]
        public bool AllowWarning { get; set; } = true;

        [Required]
        public int Optimal_Stock { get; set; } = 0;

    }
}

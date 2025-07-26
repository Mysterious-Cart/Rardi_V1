using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models
{
    public partial class ProductModel
    {
        [NotMapped]
        public int Max { get; set; } = 0;
        [NotMapped]
        public int Min { get; set; } = 0;
        [NotMapped]
        public int Sold { get; set; } = 0;
        [NotMapped]
        public int Returned { get; set; } = 0;

    }
}
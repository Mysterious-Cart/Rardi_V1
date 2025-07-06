using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHKS.Models;
public class ProductProfiles
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string Profile { get; set; }
    [Required]
    public decimal Price { get; set; }

}
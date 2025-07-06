
using System.ComponentModel.DataAnnotations;

namespace CHKS.Models;
public class Problem()
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public Guid CartId { get; set; }
    [Required]
    public string Desciption { get; set; } = "";
    [Required]
    public string Title { get; set; } = "";

}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CHKS.Models.mydb;
using Microsoft.EntityFrameworkCore;
namespace CHKS.Models;

[Table("Package")]
[PrimaryKey("Id")]

public class Package_Model
{
    [Required]
    public Guid Id { get; set; }
    public string Description { get; set; } = "";
    public IEnumerable<ProductModel> Products { get; set; }

}
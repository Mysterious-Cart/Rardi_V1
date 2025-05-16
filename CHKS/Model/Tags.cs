using CHKS.Models.Interface;
using CHKS.Models.mydb;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CHKS.Models.mydb;

[PrimaryKey("Id")]
public class Tags
{
    [Key]
    [Required]
    public Guid Id {get; set;} = Guid.NewGuid();

    public string Tag {get; set;}
    public string Color {get; set;}
    public string Description { get; set; } = "";
    public ICollection<Product_Model> Product;

}
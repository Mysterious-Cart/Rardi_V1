
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models;

[Table("Order")]
[PrimaryKey("Id")]
public class Order_Model
{
    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public Guid ProductId { get; set; }
    public Product_Model Product { get; set; }
    [Required]
    public int Amount { get; set; }
    [Required]
    [Column(TypeName = "date")]
    public DateOnly OrderDate { get; } = DateOnly.FromDateTime(DateTime.Now);
    [Required]
    public bool IsOrderReceived { get; set; } = false;
    [Required]
    public bool IsCancelled { get; set; } = false;
    [Column(TypeName = "date")]
    public DateOnly? OrderReceivedDate { get; set; } = null;
    [Column(TypeName = "date")]
    public DateOnly? DeliveryDate { get; set; } = null;
    public string Description { get; set; } = "";
    
    public decimal TotalPrice { get; set; } = 0.0m;

}
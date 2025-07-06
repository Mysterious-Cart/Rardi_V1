using System.ComponentModel.DataAnnotations;
namespace CHKS.Models;

public class UserNotificationStamp
{

    [Key]
    [Required]
    public string UserId { get; set; }
    public Aspnetuser User { get; set; }
    [Key]
    [Required]
    public Guid LogId { get; set; }
    public StockLogs Log { get; set; }
    [Timestamp]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CHKS.Models;

[PrimaryKey(nameof(UserId), nameof(LogId))]
public class UserNotificationStampModel
{

    [Key]
    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public Aspnetuser User { get; set; }
    [Key]
    [Required]
    [ForeignKey(nameof(Log))]
    public Guid LogId { get; set; }
    public StockLogsModel Log { get; set; }
    [Timestamp]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
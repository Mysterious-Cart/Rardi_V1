using CHKS.Models;
using CHKS.Data;
using Microsoft.EntityFrameworkCore;
namespace CHKS.Services;

public class StockLogsTrackingService(IDbContextFactory<Rardi_Context> contextFactory, SecurityService securityService)
{
    private readonly Rardi_Context _Context = contextFactory.CreateDbContext();
    private readonly SecurityService securityService = securityService;
    private async Task<List<UserNotificationStamp>> GetAllUserNotificationStamps()
    {
        return await _Context.NotificationStampModels.Select(i => new UserNotificationStamp(
            i.UserId,
            i.LogId
        )).ToListAsync();
    }
    private async Task<UserNotificationStamp> GetUserNotificationStamps(string UserId, Guid LogsId)
    {
        if (string.IsNullOrEmpty(UserId))
            throw new ArgumentNullException(nameof(UserId), "UserId cannot be null or empty.");

        if (LogsId == Guid.Empty)
            throw new ArgumentNullException(nameof(LogsId), "LogsId cannot be empty.");

        try
        {
            var stamps = await _Context.NotificationStampModels
                .Where(i => i.UserId == UserId && i.LogId == LogsId)
                .Select(i => new UserNotificationStamp(i.UserId, i.LogId))
                .FirstAsync();

            return stamps;
        }
        catch (InvalidOperationException)
        {
            // No matching record found
            return null;
        }
    }
    private async Task<UserNotificationStamp> AddUnseenStamp(string UserId, Guid LogsId)
    {
        var existingStamp = await GetUserNotificationStamps(UserId, LogsId);
        if (existingStamp is not null)
            return existingStamp; // Already seen

        if (!_Context.StockLogs.Any(i => i.Id == LogsId))
            throw new ArgumentException("Log does not exist.", nameof(LogsId));

        if ((await securityService.GetUserById(UserId)) is null)
            throw new ArgumentException("User does not exist.", nameof(UserId));

        var Unseen_Stamp = new UserNotificationStampModel
        {
            UserId = UserId,
            LogId = LogsId,
        };
        try
        {
            _Context.NotificationStampModels.Add(Unseen_Stamp);
            await _Context.SaveChangesAsync();
            return new UserNotificationStamp(Unseen_Stamp.UserId, Unseen_Stamp.LogId);
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Failed to add unseen stamp.", ex);
        }
    }

    public async Task GenerateUnseenStampForAllUsers(Guid LogsId)
    {

        var users = await _Context.Set<Aspnetuser>().Select(i => new { i.Id }).ToListAsync();
        foreach (var user in users)
        {
            await AddUnseenStamp(user.Id, LogsId);
        }
    }
}
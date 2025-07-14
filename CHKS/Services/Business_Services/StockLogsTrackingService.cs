using CHKS.Models.Interface;
using CHKS.Models;
namespace CHKS.Services;

public class StockLogsTrackingService : IAsyncDisposable
{
    private readonly IDbProvider _provider;
    public StockLogsTrackingService(IDbProvider provider)
    {
        _provider = provider;
    }
    private async Task Create_Unseen_Stamp(string UserId, Guid LogsId, bool safe_check = true)
    {
        if (safe_check)
        {
            if ((await _provider.GetData<UserNotificationStamp>()).Any(i => i.UserId == UserId && i.LogId == LogsId))
                return; // Already seen

            if ((await _provider.GetData<StockLogs>()).All(i => i.Id != LogsId))
                throw new ArgumentException("Log does not exist.", nameof(LogsId));

            if ((await _provider.GetData<Aspnetuser>()).All(i => i.Id != UserId))
                throw new ArgumentException("User does not exist.", nameof(UserId));
        }

        var Unseen_Stamp = new UserNotificationStamp
        {
            UserId = UserId,
            LogId = LogsId,
        };

        await _provider.CreateData(Unseen_Stamp);
    }

    public async Task Generate_Unseen_Stamp_ForAllUsers(Guid LogsId)
    {

        var users = await _provider.GetData<Aspnetuser>();
        foreach (var user in users)
        {
            await Create_Unseen_Stamp(user.Id, LogsId, false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _provider.DisposeAsync();
        GC.SuppressFinalize(this);
        GC.Collect();
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using CHKS.Entity;

namespace CHKS.Services;

public class InventoryNotificationHubConnectionService : IAsyncDisposable
{

    private readonly NavigationManager navigationManager;

    HubConnection _hubConnection;
    public event Action<StockLogs> OnProductChanged;

    public InventoryNotificationHubConnectionService(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
    }

    public async Task StartConnection()
    {

        if (_hubConnection != null) return;

        var url = navigationManager.ToAbsoluteUri("/inventorylogs").ToString();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();

        _hubConnection.On<StockLogs>("ReceiveMessage", message => OnProductChanged?.Invoke(message));

        await _hubConnection.StartAsync();
    }


    public async Task SendChangesLog(StockLogs logs)
    {
        if (_hubConnection == null) return;

        await _hubConnection.SendAsync("SendMessage", logs);
    }

    public async Task StopConnection()
    {
        if (_hubConnection == null) return;

        await _hubConnection.StopAsync();
        await _hubConnection.DisposeAsync();
        _hubConnection = null;
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await StopConnection();
    }
}
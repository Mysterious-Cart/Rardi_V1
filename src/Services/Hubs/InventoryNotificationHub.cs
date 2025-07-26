using System;
using System.Collections.Generic;
using CHKS.Entity;
using Microsoft.AspNetCore.SignalR;
using CHKS.Services;

namespace CHKS.Services
{
    public class InventoryNotificationHub : Hub
    {
        private readonly StockLogsTrackingService _stockLogsTrackingService;
        public InventoryNotificationHub(StockLogsTrackingService stockLogsTrackingService)
        {
            _stockLogsTrackingService = stockLogsTrackingService;
        }
        public async Task SendMessage(StockLogs message)
        {
            // Send a message to all connected clients
            Console.WriteLine($"Sending message to all clients: {message}");
            await _stockLogsTrackingService.GenerateUnseenStampForAllUsers(message.Id);
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        public override Task OnConnectedAsync()
        {
            // Handle when a client connects
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Handle when a client disconnects
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}-{exception?.Message}");
            return base.OnDisconnectedAsync(exception);
        }

    }
}
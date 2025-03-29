using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EPlatform_API.Helper
{
    public class NotificationHub : Hub
    {

        
        // userId -> connectionId
        private static readonly Dictionary<string, string> _connections = new();
        private readonly NotificationRepo _notificationRepo;

        public NotificationHub(NotificationRepo notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null && !_connections.ContainsKey(userId))
            {
                _connections[userId] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null && _connections.ContainsKey(userId))
            {
                _connections.Remove(userId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string[] userIds, string message)
        {
            if (userIds == null || userIds.Length == 0)
                return;
            
            userIds = userIds.Distinct().ToArray(); // Remove duplicates

            foreach (var userId in userIds)
            {
                if (_connections.TryGetValue(userId, out var connectionId))
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models.ShopOwners;
using MongoDB.Driver;

namespace EPlatform_API.Repository
{
    public class NotificationRepo : MongoRepository<ShopNotification>
    {
        private readonly IMongoCollection<ShopNotification> _shopNotificationCollection;
        public NotificationRepo(IMongoDatabase database, ILoggingService loggingService) : base(database, loggingService)
        {
            _shopNotificationCollection = database.GetCollection<ShopNotification>(MongoDbCollections.ShopNotification);
        }

        public async Task SaveNotification(List<ShopNotification> shopNotifications)
        {
            if (shopNotifications == null)
                return;

            try
            {
                Console.WriteLine($"Saving {shopNotifications.Count} notifications to the database.");
                await _shopNotificationCollection.InsertManyAsync(shopNotifications);
            }
            catch (MongoBulkWriteException<ShopNotification> ex)
            {
                // Handle the exception as needed
                Console.WriteLine($"Bulk write error: {ex.Message}");
            }
        }

        public async Task<List<ShopNotification>> GetNotificationByShopIdAsync(string shopId)
        {
            var filter = Builders<ShopNotification>.Filter.Eq(x => x.ShopId, shopId);
            var notifications = await _shopNotificationCollection.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync();

            return notifications;
        }

        // RemoveNotificationAsync
        public async Task RemoveNotificationAsync(string notificationId)
        {
            var filter = Builders<ShopNotification>.Filter.Eq(x => x._id, notificationId);
            var result = await _shopNotificationCollection.DeleteOneAsync(filter);
        }
    }
}
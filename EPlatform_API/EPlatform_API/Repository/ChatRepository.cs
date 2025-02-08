using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Models.ShopOwners;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;

namespace EPlatform_API.Repository
{
    public class ChatRepository : MongoRepository<Chat>,IChatRepository
    {
        private readonly IMongoCollection<Chat>  _chatCollections;
        public ChatRepository(IMongoDatabase database, ILoggingService loggingService) : base(database, loggingService)
        {
            _chatCollections = database.GetCollection<Chat>("Chat");
        }

        public async Task<List<Chat>> GetChatTodayOfUser(string userId)
        {
            var filter = Builders<Chat>.Filter.Eq(c => c.CustomerId, userId);
            var chats = await _chatCollections.Find(filter).ToListAsync();
            chats = chats.Where(c => UtilityServices.ConvertUTCToVietNam(c.Time) >= DateTime.Today).ToList();
            return chats;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using StackExchange.Redis;

namespace EPlatform_API.Services
{
    public class RedisServices : IRedisServices
    {
        public RedisServices()
        {
        }

        public async Task IncreaseSearchTermCount(string searchTerm)
        {
            var db = RedisManager.Connection.GetDatabase();

            if (!await db.KeyExistsAsync($"search_count:{searchTerm}"))
            {
                await db.StringSetAsync($"search_count:{searchTerm}", 0, TimeSpan.FromDays(2));
            }
            
            await db.StringIncrementAsync($"search_count:{searchTerm}");
        }
    }
}
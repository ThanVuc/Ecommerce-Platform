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
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDatabase;

        public RedisServices(
            IConnectionMultiplexer redis)
        {
            _redisConnection = redis;
            _redisDatabase = redis.GetDatabase();
        }

        public async Task IncreaseSearchTermCount(string searchTerm)
        {

            if (!await _redisDatabase.KeyExistsAsync($"search_count:{searchTerm}"))
            {
                await _redisDatabase.StringSetAsync($"search_count:{searchTerm}", 0, TimeSpan.FromDays(2));
            }
            
            await _redisDatabase.StringIncrementAsync($"search_count:{searchTerm}");
        }

        public async Task<string> GetString(string value)
        {
            var stringValue = await _redisDatabase.StringGetAsync(value);
            if (stringValue.IsNullOrEmpty)
            {
                return null;
            }
            return stringValue.ToString();
        }

        public async Task SetString(string key, string value, TimeSpan? expiry = null)
        {
            await _redisDatabase.StringSetAsync(key, value, expiry);
        }

        public async Task<string> GetOrSetString(string key, string valueFactory, TimeSpan? expiry = null)
        {
            var stringValue = await _redisDatabase.StringGetAsync(key);
            if (stringValue.IsNullOrEmpty)
            {
                await SetString(key, valueFactory, expiry);
                return valueFactory;
            }
            return stringValue.ToString();
        }
    }
}
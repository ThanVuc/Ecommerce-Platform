using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EPlatform_API.Services
{
    public class QueryingServices : IQueryingServices
    {
        private readonly AppDbContext _dbContext;
        private readonly IRedisServices _redisService;

        public QueryingServices(
            AppDbContext appDbContext,
            IRedisServices redisServices
        )
        {
            _dbContext = appDbContext;
            _redisService = redisServices;
        }

        public async Task<List<string>?> GetUserSuggestionAsync(string searchTerm)
        {
            var redis_db = RedisManager.Connection.GetDatabase();
            var usersString = await redis_db.StringGetAsync($"search_user:{searchTerm}");
            var usersSuggestionString = await redis_db.StringGetAsync($"search_user_suggest:{searchTerm}");


            if (usersString.HasValue){
                return JsonConvert.DeserializeObject<List<AppUser>>(usersString).Take(5).Select(u => u.UserName).ToList();
            }

            if (usersSuggestionString.HasValue){
                return JsonConvert.DeserializeObject<List<string>>(usersSuggestionString).Take(5).ToList();
            }

            var users = await _dbContext.Users.Where(u => u.UserName.Contains(searchTerm)).Take(5).Select(u => u.UserName).ToListAsync();
            var usersList = users.ToList();
            var usersJson = JsonConvert.SerializeObject(usersList);
            redis_db.StringSet($"search_user_suggest:{searchTerm}",usersJson,TimeSpan.FromSeconds(30));

            return users;
        }

        public async Task<IQueryable<AppUser>?> SearchUserAsync(string searchTerm)
        {
            await _redisService.IncreaseSearchTermCount(searchTerm);

            var redis_db = RedisManager.Connection.GetDatabase();
            var usersString = await redis_db.StringGetAsync($"search_user:{searchTerm}");

            if (usersString.HasValue){
                return JsonConvert.DeserializeObject<List<AppUser>>(usersString).AsQueryable();
            }

            var users = _dbContext.Users.Where(u => u.UserName.Contains(searchTerm));

            var frequence =  (int)await redis_db.StringGetAsync($"search_count:{searchTerm}");
            if (frequence >= 3){
                var usersList = users.ToList();
                var usersJson = JsonConvert.SerializeObject(usersList);
                redis_db.StringSet($"search_user:{searchTerm}",usersJson,TimeSpan.FromMinutes(10));
            }

            return users;
        }
    }
}
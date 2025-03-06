using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models.ShopOwners;
using StackExchange.Redis;

namespace EPlatform_API.Repository
{
    public class OrderRepository : RepositoryBase<Models.ShopOwners.Order>
    {
        private readonly AppDbContext _context;
        private readonly IDatabase _redis;
        public OrderRepository(AppDbContext context, IConfiguration configuration, ILoggingService loggingService) : base(context, configuration, loggingService)
        {
            _context = context;
            _redis = RedisManager.Connection.GetDatabase();
        }
    }
}
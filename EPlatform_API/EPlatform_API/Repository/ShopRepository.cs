using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.ShopDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EPlatform_API.Repository
{
    public class ShopRepository : RepositoryBase<Shop>, IShopRepository
    {
        private readonly DbSet<Shop> _shopTable;
        private readonly DbSet<AppUser> _userTable;
        private readonly IDatabase _redisDb;

        public ShopRepository(AppDbContext context, IConfiguration configuration, ILoggingService loggingService) : base(context, configuration, loggingService)
        {
            _shopTable = context.Shops;
            _userTable = context.Users;
            _redisDb = RedisManager.Connection.GetDatabase();
        }

        public Task<IEnumerable<Shop>> GetAllShopsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Shop?> GetShopByIdAsync(string shopId)
        {
            return await _shopTable.FirstOrDefaultAsync(s => s.ShopId == shopId);
        }

        public async Task<ShopDetailResponse?> GetShopResponseByIdAsync(string shopId)
        {
            var shopRedisValue = _redisDb.StringGet($"shop:{shopId}");
            if (shopRedisValue.HasValue)
            {
                return JsonConvert.DeserializeObject<ShopDetailResponse>(shopRedisValue);
            }

            var shop = await _shopTable
            .Select(s => new ShopDetailResponse
            {
                ShopId = s.ShopId,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                ShopAddress = s.ShopAddress,
                TaxesCode = s.TaxesCode,
                IdentificationNumber = s.IdentificationNumber
            })
            .FirstOrDefaultAsync(s => s.ShopId == shopId);

            if (shop == null)
            {
                return null;
            }

            var shopJson = JsonConvert.SerializeObject(shop);
            _redisDb.StringSet($"shop:{shopId}", shopJson, TimeSpan.FromDays(1));

            return shop;
        }

        public async Task<ShopLayoutResponse?> GetShopByIdlayoutAsync(string shopId)
        {
            var shopLayout = _redisDb.StringGet($"shop-layout:{shopId}");

            if (shopLayout.HasValue)
            {
                return JsonConvert.DeserializeObject<ShopLayoutResponse>(shopLayout);
            }

            var shopOwner = await _userTable
            .Select(u => new ShopLayoutResponse
            {
                ShopId = u.Id,
                Name = u.Last,
                AvatarImageUrl = u.AvatarImageUrl
            })
            .FirstOrDefaultAsync(u => u.ShopId == shopId);

            if (shopOwner == null)
            {
                return null;
            }

            var shopLayoutJson = JsonConvert.SerializeObject(shopOwner);
            _redisDb.StringSet($"shop-layout:{shopId}", shopLayoutJson, TimeSpan.FromDays(1));

            return shopOwner;
        }

        public async Task<string> GetUserIdByNameAsync(string? name)
        {
            var user = await _userTable.FirstOrDefaultAsync(u => u.UserName == name);
            if (user == null)
            {
                throw new NullReferenceException("User not found");
            }
            return user.Id;
        }
    
        public async Task CreateShopAsync(Shop shop){
            await _shopTable.AddAsync(shop);
        }
    
        public async Task<bool> IsExist(string shopId){
            var shop = await _shopTable.FirstOrDefaultAsync(s => s.ShopId == shopId);
            return shop == null ? false : true;
        }
    }
}
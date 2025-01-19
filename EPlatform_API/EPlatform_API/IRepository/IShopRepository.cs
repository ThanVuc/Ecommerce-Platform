using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ShopDTOs;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;

namespace EPlatform_API.IRepository
{
    public interface IShopRepository
    {
        Task<ShopDetailResponse?> GetShopResponseByIdAsync(string shopId);
        Task<ShopLayoutResponse?> GetShopByIdlayoutAsync(string shopId);
        Task<IEnumerable<Shop>> GetAllShopsAsync();
        Task<Shop?> GetShopByIdAsync(string shopId);
    }
}
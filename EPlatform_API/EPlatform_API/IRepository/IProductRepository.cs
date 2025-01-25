using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ShopDTOs;
using EPlatform_API.Models.ShopOwners;

namespace EPlatform_API.IRepository
{
    public interface IProductRepository
    {
        IQueryable<Product> GetProductsByShopSummerize(string shopId);
        Task DeleteProductImage(string imageId);
        Task DeleteProductImage(List<string> imageIds);
        Task AddInventoryAsync(Inventory inventory);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<List<GetCategoriesResponse>?> GetCategoriesAsync(int? parentCategoryId = null);
    }
}
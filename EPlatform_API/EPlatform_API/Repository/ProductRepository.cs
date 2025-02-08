using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.ShopDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Mappers;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EPlatform_API.Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IDatabase _redis;

        public ProductRepository(AppDbContext context, IConfiguration configuration, ILoggingService loggingService) : base(context, configuration, loggingService)
        {
            _blobServices = new BlobServices(configuration, BlogStorage.PublicImages);
            _context = context;
            _redis = RedisManager.Connection.GetDatabase();
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            if (inventory == null)
            {
                throw new Exception("Inventory is null");
            }

            
            try {
                await _context.Inventories.AddAsync(inventory);
            } catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        public async Task AddProductAsync(Product product)
        {
            if (product == null)
            {
                throw new Exception("Product is null");
            }

            try {
                await _context.Products.AddAsync(product);
            } catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        public async Task DeleteProductImage(string imageId)
        {
            var img = _context.ProductImages.FirstOrDefault(i => i.ImageId == imageId);
            if (img == null)
            {
                throw new Exception("Image not found");
            }

            img.IsDeleted = true;
            img.DeletedAt = DateTime.Now;
            _context.ProductImages.Update(img);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductImage(List<string> imageIds)
        {
            var tasks = new List<Task>();
            foreach (var imageId in imageIds)
            {
                tasks.Add(DeleteProductImage(imageId));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<List<GetCategoriesResponse>?> GetCategoriesAsync(int? parentCategoryId = null, string? searchString = null)
        {
            IQueryable<Category> categoryQueryable = _context.Categories.AsQueryable();
            
            categoryQueryable = _context.Categories
            .Where(c => c.CategoryParentId == parentCategoryId);

            if (searchString != null)
            {
                categoryQueryable = categoryQueryable
                .Where(c => c.Name.Contains(searchString));
            }

            var categories = await categoryQueryable
            .Include(c => c.SubCategories)
            .Select(c => c.ToCategoriesResponse())
            .ToListAsync();
            return categories;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return null;
            }

            return product;
        }

        public IQueryable<Product> GetProductsByShopSummerize(string shopId)
        {
            var products = _context.Products
            .Where(p => p.ShopId == shopId)
            .Select(p => new Product
            {
                ProductId = p.ProductId,
                AvtImgUrl = p.AvtImgUrl,
                Price = p.Price,
                IsPublic = p.IsPublic,
                Name = p.Name,
                Inventory = new Inventory
                {
                    AvailableQuantity = p.Inventory == null ? 0 : p.Inventory.AvailableQuantity,
                    IsAvailable = p.Inventory == null ? false : p.Inventory.IsAvailable
                },
                Slug = p.Slug
            })
            .AsNoTracking()
            .AsQueryable();

            return products;
        }
    
        public async Task AddProductImagesAsync(int productId, Dictionary<string,string> productImgDict)
        {
            // Add cover image
            await _context.ProductImages.AddAsync(new ProductImage
            {
                ProductId = productId,
                ImgType = "WebP",
                ImgUrl = productImgDict["coverImage"],
                IsPrimary = true,
                DeletedAt = null,
                IsDeleted = false,
                UploadAt = DateTime.Now
            });

            // add other images
            foreach (var img in productImgDict)
            {
                if (img.Key == "coverImage")
                {
                    continue;
                }

                await _context.ProductImages.AddAsync(new ProductImage
                {
                    ProductId = productId,
                    ImgType = "WebP",
                    ImgUrl = img.Value,
                    IsPrimary = false,
                    DeletedAt = null,
                    IsDeleted = false,
                    UploadAt = DateTime.Now
                });
            }
        }
    }
}
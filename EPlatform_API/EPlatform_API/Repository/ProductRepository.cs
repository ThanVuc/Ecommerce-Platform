using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.ProductDTOs;
using EPlatform_API.DTOs.ShopDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Mappers;
using EPlatform_API.Models;
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
            _blobServices = new BlobServices(configuration, BlobStorage.PublicImages);
            _context = context;
            _redis = RedisManager.Connection.GetDatabase();
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            if (inventory == null)
            {
                throw new Exception("Inventory is null");
            }


            try
            {
                await _context.Inventories.AddAsync(inventory);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task AddProductAsync(Product product)
        {
            if (product == null)
            {
                throw new Exception("Product is null");
            }

            try
            {
                await _context.Products.AddAsync(product);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
            var product = await _context.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return null;
            }

            return product;
        }

        public async Task<Product?> GetProductAllByIdAsync(int productId)
        {
            var product = await _context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return null;
            }

            return product;
        }

        public IQueryable<Product> GetProductsByShopSummerize(string shopId)
        {
            var products = _context.Products
            .Where(p => p.ShopId == shopId && p.IsDeleted == false)
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
        public async Task<bool> UpdateProduct(int productId ,AddProductRequest updateProductModel, ImageStoreModel? imageStoreModel)
        {
            var product = await _context.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return false;
            }

            if (product.Inventory == null)
            {
                throw new Exception("Inventory is null");
            }

            product.Name = updateProductModel.Name;
            product.Price = updateProductModel.Price;
            product.Description = updateProductModel.Description;
            product.CategoryId = updateProductModel.CategoryId;
            product.IsPublic = updateProductModel.IsPublic;
            if (imageStoreModel != null)
            {
                product.AvtImgUrl = imageStoreModel.Url;
                product.AvtImgName = imageStoreModel.Name;
            }
            product.Inventory.Quantity = updateProductModel.TotalInventory;
            product.Inventory.AvailableQuantity = updateProductModel.TotalInventory - product.Inventory.SoldQuantity;
            product.Inventory.IsAvailable = product.Inventory.AvailableQuantity > 0;
            product.UpdatedAt = DateTime.Now;
            product.Inventory.WareHouseId = updateProductModel.WarehouseId;
            product.IsPublic = updateProductModel.IsPublic;

            _context.Products.Update(product);
            return true;
        }
        public bool DeleteProductByIdAsync(int productId)
        {
            var product = _context.Products
            .FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("Product is not found");
            }
            product.IsDeleted = true;
            product.DeletedAt = DateTime.Now;
            _context.Products.Update(product);
            return true;
        }
    
        // ----------------------
        public async Task<List<object>> GetCategoriesInHome()
        {
            var categories = await _context.Categories
            .Where(c => c.CategoryParentId == null)
            .Select(c => new 
            {
                c.CategoryId,
                c.Name,
                c.ImgUrl
            })
            .ToListAsync();

            return categories.Cast<object>().ToList();
        }
    
        public async Task<List<object>> GetHotProducts()
        {
            var beginThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var products = await _context.Products
            .Include(p => p.Inventory)
            .Where(p => p.IsPublic == true && p.CreatedAt >= beginThisMonth)
            .OrderByDescending(p => p.Inventory.SoldQuantity)
            .Select(p => new 
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                AvtImgUrl = p.AvtImgUrl,
                SoldQuantity = UtilityServices.ConvertBigNumberToShortNumber((long)p.Inventory.SoldQuantity),
                Slug = p.Slug
            })
            .Take(20)
            .ToListAsync();

            if (products.Count < 5){
                var temp = await _context.Products
                .Include(p => p.Inventory)
                .Where(p => p.IsPublic == true)
                .OrderByDescending(p => p.Inventory.SoldQuantity)
                .Select(p => new 
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    AvtImgUrl = p.AvtImgUrl,
                    SoldQuantity = UtilityServices.ConvertBigNumberToShortNumber((long)p.Inventory.SoldQuantity),
                    Slug = p.Slug
                })
                .Take(20 - products.Count)
                .ToListAsync();
                products.AddRange(temp);
            }

            return products.Cast<object>().ToList();
        }

        public async Task<List<object>> GetTodaySuggestions()
        {
            // take day begin week, monday
            var beginThisWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + (int)DayOfWeek.Monday);

            var products = await _context.Products
            .Include(p => p.Inventory)
            // .Where(p => p.IsPublic == true && p.CreatedAt >= beginThisWeek)
            .Where(p => p.IsPublic)
            .OrderByDescending(p => p.Inventory.SoldQuantity)
            .Select(p => new 
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                AvtImgUrl = p.AvtImgUrl,
                SoldQuantity = UtilityServices.ConvertBigNumberToShortNumber((long)p.Inventory.SoldQuantity),
                Slug = p.Slug
            })
            .Take(20)
            .ToListAsync();

            return products.Cast<object>().ToList();
        }

        public async Task<Product> GetProductById(int productId)
        {
            var product = await _context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Category)
            .ThenInclude(c => c.ParentCategory)
            .Include(p => p.Shop)
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsPublic == true);

            if (product == null)
            {
                throw new Exception("Product is not found");
            }

            return product;
        }
    }
}
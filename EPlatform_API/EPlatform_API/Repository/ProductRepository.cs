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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EPlatform_API.Repository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IDatabase _redis;

        public ProductRepository(
            AppDbContext context, 
            IConfiguration configuration, 
            ILoggingService loggingService
        ) : base(context, configuration, loggingService)
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

        public async Task<Product?> GetProductByIdAsync(int productId, string shopId)
        {
            var product = await _context.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDeleted == false);

            if (product == null)
            {
                return null;
            }

            if (product.ShopId != shopId)
            {
                throw new Exception("Product is not belong to this shop");
            }

            return product;
        }

        public async Task<Product?> GetProductAllByIdAsync(int productId, string shopId)
        {
            var product = await _context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDeleted == false);

            if (product == null)
            {
                return null;
            }

            if (product.ShopId != shopId)
            {
                throw new Exception("Product is not belong to this shop");
            }

            return product;
        }

        public IQueryable<Product> GetProductsByShopSummerize(string shopId)
        {
            var products = _context.Products
            .Where(p => p.ShopId == shopId && p.IsDeleted == false)
            .OrderByDescending(p => p.CreatedAt)
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
        public async Task<bool> UpdateProduct(int productId, AddProductRequest updateProductModel, ImageStoreModel? imageStoreModel, string shopId)
        {
            var product = await _context.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return false;
            }

            if (product.ShopId != shopId)
            {
                throw new Exception("Product is not belong to this shop");
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
        public bool DeleteProductByIdAsync(int productId, string shopId)
        {
            var product = _context.Products
            .FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                throw new Exception("Product is not found");
            }

            if (product.ShopId != shopId)
            {
                throw new Exception("Product is not belong to this shop");
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
            // .Where(p => p.IsPublic == true && p.CreatedAt >= beginThisMonth)
            .Where(p => p.IsPublic == true && p.Inventory.IsAvailable == true && p.IsDeleted == false)
            // .OrderByDescending(p => p.Inventory.SoldQuantity)
            .OrderByDescending(p => p.CreatedAt)
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

            if (products.Count < 5)
            {
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
            .Where(p => p.IsPublic && p.Inventory.IsAvailable == true && p.IsDeleted == false)
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

        public async Task<Cart> GetCart(string userId){

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == userId);

            if (cart != null){
                return cart;
            }

            var cart_new = new Cart
            {
                CustomerId = userId
            };

            await _context.Carts.AddAsync(cart_new);
            await _context.SaveChangesAsync();
            return cart_new;
        }

        public async Task<CartItem> AddItemToCart(string customerName ,AddItemToCart addItemToCart)
        {;
            var customerId = RetriveUserIdFromName(customerName);
            var cart = await GetCart(customerId);

            var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == addItemToCart.ProductId && ci.SpecInfo == addItemToCart.SpecInfo);

            if (cartItem != null)
            {
                cartItem.Quantity += addItemToCart.Quantity;
                _context.CartItems.Update(cartItem);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = addItemToCart.ProductId,
                    Quantity = addItemToCart.Quantity,
                    SpecInfo = addItemToCart.SpecInfo,
                    CreatedAt = DateTime.Now
                };

                await _context.CartItems.AddAsync(newCartItem);
                return newCartItem;
            }

            return cartItem;
        }

        public async Task<List<CartItemsResponse>> GetCartItems(string customerName)
        {
            var customerId = RetriveUserIdFromName(customerName);
            var cartId = (await GetCart(customerId)).CartId;

            var cartItems = await _context.CartItems
            .Include(ci => ci.Product)
            .ThenInclude(p => p.Inventory)
            .Include(ci => ci.Product)
            .ThenInclude(p => p.Shop)
            .Where(ci => ci.CartId == cartId)
            .Select(ci => new CartItemsResponse {
                CartItemId = ci.CartItemId,
                ProductName = ci.Product.Name,
                ProductPrice = ci.Product.Price,
                AvailableQuantity = 0,
                Quantity = ci.Quantity,
                ProductAvtImg = ci.Product.AvtImgUrl,
                ProductId = ci.ProductId,
                SpecInfo = ci.SpecInfo,
                ShopId = ci.Product.ShopId,
                ShopName = ci.Product.Shop.Name,
                ShopLogoUrl = ci.Product.Shop.LogoUrl
            })
            .ToListAsync();
            

            return cartItems;
        }

        public async Task<int> GetCartItemsCount(string customerName)
        {
            var customerId = RetriveUserIdFromName(customerName);
            var cartId = (await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == customerId))?.CartId;

            if (cartId == null)
            {
                return 0;
            }

            var cartItemsCount = await _context.CartItems.Where(ci => ci.CartId == cartId).CountAsync();
            return cartItemsCount;
        }

        public string RetriveUserIdFromName(string name)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == name);
            if (user == null)
            {
                throw new Exception("User is not found");
            }
            return user.Id;
        }

        public Task RemoveCartItems(int cartItemId){
            _context.CartItems.Remove(new CartItem { CartItemId = cartItemId });
            return Task.CompletedTask;
        }
    
        public async Task MinusProductInventory(List<CartItemOfOrder> cartItemOfOrders)
        {
            try {
                foreach (var cartItem in cartItemOfOrders)
                {
                    var product = _context.Products
                    .Include(p => p.Inventory)
                    .FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                    if (product == null)
                    {
                        throw new Exception("Product is not found");
                    }

                    if (product.Inventory == null)
                    {
                        throw new Exception("Product is not found");
                    }
                    product.Inventory.AvailableQuantity -= cartItem.Quantity;
                    product.Inventory.SoldQuantity += cartItem.Quantity;

                    if (product.Inventory.AvailableQuantity <= 0)
                    {
                        product.Inventory.IsAvailable = false;
                    }

                    _context.Products.Update(product);
                }
            } catch(Exception e) {
                throw new Exception($"Error when minus product inventory: {e.Message}");
            }
            
        }
    
        public async Task ClearCart(List<CartItemOfOrder> cartItemOfOrders)
        {
            foreach (var cartItem in cartItemOfOrders)
            {
                var cartItemEntity = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItem.CartItemId);

                if (cartItemEntity != null)
                {
                    _context.CartItems.Remove(cartItemEntity);
                }
            }
        }
    
        // SearchProducts
        public IQueryable<SearchProductsReponse>? SearchProducts(string? searchString, int categoryId)
        {
            try {
                var products = _context.Products
                .Include(p => p.Inventory)
                .Include(p => p.Category)
                .Where(p => p.IsPublic == true && p.IsDeleted == false)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => p.ToSearchProductResponse())
                .AsNoTracking()
                .AsQueryable();

                if (products == null)
                {
                    return null;
                }

                if (searchString != null)
                {
                    products = products.AsEnumerable()
                        .Where(p => p.Name != null && p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        .AsQueryable();
                }

                if (categoryId != 0)
                {
                    products = products
                    .AsEnumerable() 
                    .Where(p => p.CategoryId == categoryId).AsQueryable();
                }

                return products;
            } catch(Exception e) {
                throw new Exception($"Error when search products: {e.Message}");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bogus;
using EPlatform_API.Data;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EPlatform_API.Services
{
    public class SeedDataService : ISeedDataService
    {
        private readonly AppDbContext _context;
        private readonly VietnameseLocationContext _vietnameseLocationContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ProductInfoMongoRepository _productInfoMongoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SeedDataService(
            AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            VietnameseLocationContext vietnameseLocationContext,
            ProductInfoMongoRepository productInfoMongoRepository,
            IUnitOfWork unitOfWork
        )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _vietnameseLocationContext = vietnameseLocationContext;
            _productInfoMongoRepository = productInfoMongoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task SeedShopData()
        {
            try
            {
                _context.Database.BeginTransaction();

                _context.Shops.RemoveRange(_context.Shops.Where(shop => shop.Name.Contains("@fake-data")));
                _context.Products.RemoveRange(_context.Products.Where(p => p.Name.Contains("@fake-data")));

                _context.SaveChanges();

                var faker = new Faker();
                var products = new List<Product>();
                var productSpecInfos = new List<ProductSpecInfo>();
                var categories = new List<Category>();

                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == "1111111111");

                // Admin shop
                var shop = new Shop
                {
                    ShopId = admin.Id,
                    Name = "Sinh Nguyen" + "@fake-data",
                    Description = faker.Company.CatchPhrase(),
                    LogoUrl = faker.Image.PicsumUrl(),
                    Rating = faker.Random.Decimal(1, 5),
                    ReviewCount = faker.Random.Int(0, 1000),
                    FollowersCount = faker.Random.Int(0, 10000),
                    CreatedAt = faker.Date.Past(),
                    UpdatedAt = faker.Date.Recent(),
                    Slug = UtilityServices.GenerateSlug("Sinh Nguyen"),
                    ShopAddress = faker.Address.FullAddress(),
                    Phone = faker.Phone.PhoneNumber("###########"),
                    Email = faker.Internet.Email(),
                    TaxesCode = faker.Random.String2(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"),
                    IdentificationNumber = faker.Random.String2(12, "0123456789"),
                    ShopOwner = admin // Assuming you will set this later
                };

                await _userManager.AddToRoleAsync(admin, RoleStorage.ShopOwner);

                await _context.Shops.AddAsync(shop);
                await _context.SaveChangesAsync();

                // Fake products
                categories = await _context.Categories.ToListAsync();
                var warehouse = await _context.Warehouses
                .Select(w => w.WarehouseId)
                .ToListAsync();

                for (int i = 0; i < 90; i++)
                {
                    var quantity = faker.Random.Int(51, 100);
                    var reservedQuantity = faker.Random.Int(0, 50);
                    var name = faker.Commerce.ProductName();
                    var cate = categories[faker.Random.Int(0, categories.Count - 1)];
                    var product = new Product
                    {
                        CategoryId = cate.CategoryId,
                        ShopId = shop.ShopId,
                        Name = name,
                        Description = faker.Commerce.ProductDescription(),
                        Price = faker.Random.Decimal(1, 1000),
                        Slug = UtilityServices.GenerateSlug(name),
                        AvtImgUrl = "https://sinhnguyen417.blob.core.windows.net/public-images/600x400.png",
                        IsPublic = faker.Random.Bool(),
                        CreatedAt = faker.Date.Recent(),
                        UpdatedAt = faker.Date.Recent(),
                        Inventory = new Inventory
                        {
                            Quantity = quantity,
                            SoldQuantity = reservedQuantity,
                            AvailableQuantity = quantity - reservedQuantity,
                            UpdatedAt = faker.Date.Recent(),
                            IsAvailable = faker.Random.Bool(),
                            WareHouseId = warehouse[faker.Random.Int(0, warehouse.Count - 1)]
                        }
                    };
                    products.Add(product);
                }

                // My products

                for (int i = 90; i < 100; i++)
                {
                    var quantity = faker.Random.Int(51, 100);
                    var reservedQuantity = faker.Random.Int(0, 50);
                    var name = faker.Commerce.ProductName();
                    var cate = categories[faker.Random.Int(0, categories.Count - 1)];
                    var product = new Product
                    {
                        CategoryId = cate.CategoryId,
                        ShopId = "1111111111",
                        Name = name,
                        Description = faker.Commerce.ProductDescription(),
                        Price = faker.Random.Decimal(1, 1000),
                        Slug = UtilityServices.GenerateSlug(name),
                        AvtImgUrl = "https://sinhnguyen417.blob.core.windows.net/public-images/600x400.png",
                        IsPublic = faker.Random.Bool(),
                        CreatedAt = faker.Date.Past(),
                        UpdatedAt = faker.Date.Recent(),
                        Inventory = new Inventory
                        {
                            Quantity = quantity,
                            SoldQuantity = reservedQuantity,
                            AvailableQuantity = quantity - reservedQuantity,
                            UpdatedAt = faker.Date.Recent(),
                            IsAvailable = faker.Random.Bool(),
                            WareHouseId = 1
                        }
                    };
                    products.Add(product);
                }

                // My product spec info
                foreach (var product in products)
                {
                    var productSpecInfo = new ProductSpecInfo
                    {
                        ProductId = product.ProductId,
                        SpecInfos = new List<Spec>(){
                        new Spec{
                            SpecName = "Color",
                            IsPrimary = true,
                            SpecItems = new List<SpecItem>(){
                                new SpecItem{
                                    SpecValue = "Sliver",
                                    SpecImageUrl = "https://sinhnguyen417.blob.core.windows.net/public-images/600x400.png"
                                }
                            }
                        }
                    },
                        SpecInfoInventories = new List<SpecInventory>(){
                        new SpecInventory{
                            PrimarySpecValueName = "Sliver",
                            SubSpecValueName = null,
                            Inventory = product?.Inventory?.Quantity == null ? 0 : (int)product.Inventory.Quantity
                        }
                    }
                    };
                    productSpecInfos.Add(productSpecInfo);
                }

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
                await _productInfoMongoRepository.CreateManyAsync(productSpecInfos);
                
                _context.Database.CommitTransaction();
            } catch (Exception e)
            {
                _context.Database.RollbackTransaction();
                Console.WriteLine("SeedDataService, SeedShopData: ", e.Message);
            }


        }

        public async Task SeedRoleData()
        {
            var faker = new Faker();
            var adminRole = await _roleManager.FindByNameAsync(RoleStorage.Admin);
            var customerRole = await _roleManager.FindByNameAsync(RoleStorage.Customer);
            var shopOwner = await _roleManager.FindByNameAsync(RoleStorage.ShopOwner);

            if (adminRole != null) await _roleManager.DeleteAsync(adminRole);
            if (customerRole != null) await _roleManager.DeleteAsync(customerRole);
            if (shopOwner != null) await _roleManager.DeleteAsync(shopOwner);

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = faker.Random.Guid().ToString(), Name = RoleStorage.Admin, NormalizedName = RoleStorage.Admin.ToUpper() },
                new IdentityRole { Id = faker.Random.Guid().ToString(), Name = RoleStorage.Customer, NormalizedName = RoleStorage.Customer.ToUpper() },
                new IdentityRole { Id = faker.Random.Guid().ToString(), Name = RoleStorage.ShopOwner, NormalizedName = RoleStorage.ShopOwner.ToUpper() },
            };

            _context.Roles.AddRange(roles);
            _context.SaveChanges();

            var admin = await _roleManager.FindByNameAsync("Admin");

            if (admin != null)
            {
                await _roleManager.AddClaimAsync(admin, new Claim("CanManipulateRolePage", "true"));
                await _roleManager.AddClaimAsync(admin, new Claim("CanManipulateUserPage", "true"));
            }
        }

        public async Task SeedUserData()
        {
            _context.Users.RemoveRange(_context.Users.Where(user => user.UserName.Contains("@fake-data")));
            _context.Shops.RemoveRange(_context.Shops.Where(shop => shop.Name.Contains("@fake-data")));
            _context.Products.RemoveRange(_context.Products.Where(p => p.Name.Contains("@fake-data")));
            _context.Categories.RemoveRange(_context.Categories.Where(c => c.Name.Contains("@fake-data")));


            var rm_admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == "1111111111");

            if (rm_admin != null) _context.Users.Remove(rm_admin);

            _context.SaveChanges();

            var faker = new Faker();
            var users = new List<AppUser>();
            var passwordHash = new PasswordHasher<AppUser>();

            var admin = new AppUser
            {
                Id = "1111111111",
                HomeAddress = faker.Address.FullAddress(),
                UserName = "sinhhahaha1@gmail.com",
                NormalizedUserName = "sinhhahaha1@gmail.com".ToUpper(),
                Email = "sinhhahaha1@gmail.com",
                NormalizedEmail = "sinhhahaha1@gmail.com".ToUpper(),
                EmailConfirmed = faker.Random.Bool(),
                SecurityStamp = faker.Random.Hash(),
                ConcurrencyStamp = faker.Random.Hash(),
                PhoneNumber = faker.Phone.PhoneNumber(),
                PhoneNumberConfirmed = faker.Random.Bool(),
                TwoFactorEnabled = faker.Random.Bool(),
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = faker.Random.Int(0, 2),
                First = faker.Name.FirstName(),
                Last = faker.Name.LastName(),
                AvatarImageUrl = "",
                Create = faker.Date.Recent(),
                Age = faker.Random.Int(18, 65),
                National = faker.Address.Country(),
                Gender = faker.Random.Bool()
            };

            admin.PasswordHash = passwordHash.HashPassword(admin, "string");

            users.Add(admin);

            for (int i = 0; i < 99; i++)
            {
                var user = new AppUser
                {
                    Id = faker.Random.Guid().ToString(),
                    HomeAddress = faker.Address.FullAddress(),
                    UserName = faker.Internet.UserName() + "@fake-data",
                    NormalizedUserName = faker.Internet.UserName().ToUpper(),
                    Email = faker.Internet.Email(),
                    NormalizedEmail = faker.Internet.Email().ToUpper(),
                    EmailConfirmed = faker.Random.Bool(),
                    PasswordHash = faker.Random.Hash(),
                    SecurityStamp = faker.Random.Hash(),
                    ConcurrencyStamp = faker.Random.Hash(),
                    PhoneNumber = faker.Phone.PhoneNumber(),
                    PhoneNumberConfirmed = faker.Random.Bool(),
                    TwoFactorEnabled = faker.Random.Bool(),
                    LockoutEnd = null,
                    LockoutEnabled = true,
                    AccessFailedCount = faker.Random.Int(0, 2),
                    First = faker.Name.FirstName(),
                    Last = faker.Name.LastName(),
                    AvatarImageUrl = "",
                    Create = faker.Date.Recent(),
                    Age = faker.Random.Int(18, 65),
                    National = faker.Address.Country(),
                    Gender = faker.Random.Bool()
                };

                users.Add(user);
            }

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(admin, "Admin");

        }

        public async Task SeedVietnameseLocationData()
        {
            try
            {
                var sqlScript = System.IO.File.ReadAllText("Sqls/ImportData_vn_units.sql");
                await _vietnameseLocationContext.Database.ExecuteSqlRawAsync(sqlScript);
            }
            catch (Exception e)
            {
                Console.WriteLine("SeedDataServices, SeedVietnameseLocationData: ", e.Message);
            }
        }

        public async Task SeedCategoryData()
        {
            try
            {
                var categoriesSqlScript = System.IO.File.ReadAllText("Sqls/category.sql");
                var subCategorySqlScript = System.IO.File.ReadAllText("Sqls/sub-category.sql");
                await _context.Database.ExecuteSqlRawAsync(categoriesSqlScript);
                // Only true with the first time seed data when create datebase
                // because the sub-category seed by id of category
                await _context.Database.ExecuteSqlRawAsync(subCategorySqlScript);
            }
            catch (Exception e)
            {
                Console.WriteLine("SeedDataServices, SeedVietnameseLocationData: ", e.Message);
            }
        }

        public async Task SeedDatabase()
        {
            await _context.Database.MigrateAsync();
        }
    }
}
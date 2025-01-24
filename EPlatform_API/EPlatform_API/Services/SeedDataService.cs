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

        public SeedDataService(
            AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            VietnameseLocationContext vietnameseLocationContext
        )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _vietnameseLocationContext = vietnameseLocationContext;
        }

        public async Task SeedShopData()
        {
            _context.Shops.RemoveRange(_context.Shops.Where(shop => shop.Name.Contains("@fake-data")));
            _context.Products.RemoveRange(_context.Products.Where(p => p.Name.Contains("@fake-data")));
            _context.Categories.RemoveRange(_context.Categories.Where(c => c.Name.Contains("@fake-data")));

            _context.SaveChanges();

            var faker = new Faker();
            var shops = new List<Shop>();
            var products = new List<Product>();
            var categories = new List<Category>();

            var users = await _context.Users
            .Where(u => u.Id != "1111111111")
            .ToListAsync();

            // Admin shop

            shops.Add(new Shop
            {
                ShopId = "1111111111",
                Name = "Sinh Nguyen" + "@fake-data",
                Description = faker.Company.CatchPhrase(),
                LogoUrl = faker.Image.PicsumUrl(),
                Rating = faker.Random.Decimal(1, 5),
                ReviewCount = faker.Random.Int(0, 1000),
                FollowersCount = faker.Random.Int(0, 10000),
                CreatedAt = faker.Date.Past(),
                UpdatedAt = faker.Date.Recent(),
                Slug = UtilityServices.GenerateSlug("Sinh Nguyen"),
                PickUpAddress = faker.Address.FullAddress(),
                ShopAddress = faker.Address.FullAddress(),
                Phone = faker.Phone.PhoneNumber("###########"),
                Email = faker.Internet.Email(),
                InvoiceEmail = faker.Internet.Email(),
                TaxesCode = faker.Random.String2(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"),
                IdentificationNumber = faker.Random.String2(12, "0123456789"),
                ShopOwner = null // Assuming you will set this later
            });

            // Fake shops

            for (int i = 0; i < 90; i++)
            {
                var name = faker.Company.CompanyName() + "@fake-data";
                var shop = new Shop
                {
                    ShopId = users[i].Id,
                    Name = name,
                    Description = faker.Company.CatchPhrase(),
                    LogoUrl = faker.Image.PicsumUrl(),
                    Rating = faker.Random.Decimal(1, 5),
                    ReviewCount = faker.Random.Int(0, 1000),
                    FollowersCount = faker.Random.Int(0, 10000),
                    CreatedAt = faker.Date.Past(),
                    UpdatedAt = faker.Date.Recent(),
                    Slug = UtilityServices.GenerateSlug(name),
                    PickUpAddress = faker.Address.FullAddress(),
                    ShopAddress = faker.Address.FullAddress(),
                    Phone = faker.Phone.PhoneNumber("###########"),
                    Email = faker.Internet.Email(),
                    InvoiceEmail = faker.Internet.Email(),
                    TaxesCode = faker.Random.String2(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"),
                    IdentificationNumber = faker.Random.String2(12, "0123456789"),
                    ShopOwner = null // Assuming you will set this later
                };
                shops.Add(shop);
            }

            await _context.Shops.AddRangeAsync(shops);
            await _context.SaveChangesAsync();

            // Fake Categories

            for (int i = 0; i < 3; i++)
            {
                var name = faker.Commerce.Categories(1)[0] + "@fake-data";
                var category = new Category
                {
                    Name = name,
                    Description = faker.Commerce.Department(),
                    CreatedAt = faker.Date.Past(),
                    UpdatedAt = faker.Date.Recent(),
                    Slug = UtilityServices.GenerateSlug(name)
                };
                categories.Add(category);
            }

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Fake products
            categories = await _context.Categories.ToListAsync();

            for (int i = 0; i < 90; i++)
            {
                var quantity = faker.Random.Int(51, 100);
                var reservedQuantity = faker.Random.Int(0, 50);
                var name = faker.Commerce.ProductName();
                var cate = categories[faker.Random.Int(0, categories.Count - 1)];
                var product = new Product
                {
                    CategoryId = cate.CategoryId,
                    ShopId = shops[faker.Random.Int(0, shops.Count - 1)].ShopId,
                    Name = name,
                    Description = faker.Commerce.ProductDescription(),
                    Price = faker.Random.Decimal(1, 1000),
                    Slug = UtilityServices.GenerateSlug(name),
                    Code = cate.Name.Substring(0,3).ToUpper(),
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
                    Code = cate.Name.Substring(0,3).ToUpper(),
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

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }

        public async Task SeedRoleData()
        {
            var faker = new Faker();
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var customerRole = await _roleManager.FindByNameAsync("Customer");

            if (adminRole != null) await _roleManager.DeleteAsync(adminRole);
            if (customerRole != null) await _roleManager.DeleteAsync(customerRole);

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = faker.Random.Guid().ToString(), Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = faker.Random.Guid().ToString(), Name = "Customer", NormalizedName = "CUSTOMER" }
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
            try{
                var sqlScript = System.IO.File.ReadAllText("Sqls/ImportData_vn_units.sql");
                await _vietnameseLocationContext.Database.ExecuteSqlRawAsync(sqlScript);
            } catch(Exception e){
                Console.WriteLine("SeedDataServices, SeedVietnameseLocationData: ", e.Message);
            }
        }

        public async Task SeedCategoryData()
        {
            try{
                var categoriesSqlScript = System.IO.File.ReadAllText("Sqls/category.sql");
                var subCategorySqlScript = System.IO.File.ReadAllText("Sqls/sub-category.sql");
                await _context.Database.ExecuteSqlRawAsync(categoriesSqlScript);
                await _context.Database.ExecuteSqlRawAsync(subCategorySqlScript);
            } catch(Exception e){
                Console.WriteLine("SeedDataServices, SeedVietnameseLocationData: ", e.Message);
            }
        }

        public async Task SeedDatabase()
        {
            await _context.Database.MigrateAsync();
        }
    }
}
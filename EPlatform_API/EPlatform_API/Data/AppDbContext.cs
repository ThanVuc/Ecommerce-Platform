using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Models.Regions;

namespace EPlatform_API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer();
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.Entity<ProductDiscount>().HasKey(pd => new { pd.ProductId, pd.DiscountId });
            builder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId });

            builder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AppUser>()
            .HasOne(u => u.Shop)
            .WithOne(s => s.ShopOwner)
            .HasForeignKey<AppUser>(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Shop>()
            .HasOne(s => s.ShopOwner)
            .WithOne(u => u.Shop)
            .HasForeignKey<Shop>(s => s.ShopId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
            .HasOne(o => o.Shipment)
            .WithOne(s => s.Order)
            .HasForeignKey<Order>(o => o.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Shipment>()
            .HasOne(s => s.Order)
            .WithOne(o => o.Shipment)
            .HasForeignKey<Shipment>(s => s.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

            
        }

        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<WareHouse> Warehouses { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentCarrier> ShipmentCarrier { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
    }
}
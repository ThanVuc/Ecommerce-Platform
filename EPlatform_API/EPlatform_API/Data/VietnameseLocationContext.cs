using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models.Regions;
using Microsoft.EntityFrameworkCore;

namespace EPlatform_API.Data
{
    public class VietnameseLocationContext : DbContext
    {
        public VietnameseLocationContext(DbContextOptions<VietnameseLocationContext> options) : base(options)
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

            builder.Entity<AdministrativeRegion>()
                .HasKey(ar => ar.Id);

            // Configurations for AdministrativeUnit
            builder.Entity<AdministrativeUnit>()
                .HasKey(au => au.Id);

            // Configurations for Province
            builder.Entity<Province>()
                .HasKey(p => p.Code);

            builder.Entity<Province>()
                .HasOne(p => p.AdministrativeUnit)
                .WithMany()
                .HasForeignKey(p => p.AdministrativeUnitId);

            builder.Entity<Province>()
                .HasOne(p => p.AdministrativeRegion)
                .WithMany()
                .HasForeignKey(p => p.AdministrativeRegionId);

            // Configurations for District
            builder.Entity<District>()
                .HasKey(d => d.Code);

            builder.Entity<District>()
                .HasOne(d => d.Province)
                .WithMany()
                .HasForeignKey(d => d.ProvinceCode);

            builder.Entity<District>()
                .HasOne(d => d.AdministrativeUnit)
                .WithMany()
                .HasForeignKey(d => d.AdministrativeUnitId);

            // Configurations for Ward
            builder.Entity<Ward>()
                .HasKey(w => w.Code);

            builder.Entity<Ward>()
                .HasOne(w => w.District)
                .WithMany()
                .HasForeignKey(w => w.DistrictCode);

            builder.Entity<Ward>()
                .HasOne(w => w.AdministrativeUnit)
                .WithMany()
                .HasForeignKey(w => w.AdministrativeUnitId);
        }

        public DbSet<AdministrativeRegion> AdministrativeRegions { get; set; }
        public DbSet<AdministrativeUnit> AdministrativeUnits { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
    }
}
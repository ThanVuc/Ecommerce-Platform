using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EPlatform_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GroupOfRole>(entity => {
                entity.HasKey(gr => new {gr.GroupID, gr.RoleID});
            });

            builder.Entity<Users>(entity => {
                entity.HasIndex(u => new {u.Username, u.Email, u.PhoneNumber});
            });
        }

        public DbSet<Users> Users {get; set;}
        public DbSet<Group> Groups {get; set;}
        public DbSet<Roles> Roles {get; set;}
        public DbSet<GroupOfRole> GroupOfRoles {get; set;}
    }
}
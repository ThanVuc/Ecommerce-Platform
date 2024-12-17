using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using EPlatform_API.Data;
using EPlatform_API.IServices;
using EPlatform_API.Models;

namespace EPlatform_API.Services
{
    public class SeedDataService : ISeedDataService
    {
        private readonly AppDbContext _context;
        public SeedDataService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedUserData()
        {
            _context.Users.RemoveRange(_context.Users.Where(user => user.UserName.Contains("@fake-data")));
            var faker = new Faker();
            var users = new List<AppUser>();

            for (int i = 0; i < 100; i++)
            {
                var user = new AppUser
                {
                    Id = faker.Random.Guid().ToString(),
                    HomeAddress = faker.Address.FullAddress(),
                    UserName = faker.Internet.UserName()+"@fake-data",
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
        }
    }
}
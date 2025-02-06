using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.IServices
{
    public interface ISeedDataService
    {
        public Task SeedUserData();
        public Task SeedShopData();

        Task SeedRoleData();
        Task SeedVietnameseLocationData();
        Task SeedDatabase();
        Task SeedCategoryData();
    }
}
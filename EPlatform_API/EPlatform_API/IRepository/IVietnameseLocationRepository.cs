using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models.Regions;

namespace EPlatform_API.IRepository
{
    public interface IVietnameseLocationRepository
    {
        Task<IList<Province>> GetProvincesAsync();
        Task<IList<District>> GetDistrictsByProvinceAsync(string provinceId);
        Task<IList<Ward>> GetWardsByDistrictAsync(string districtId);
    }
}
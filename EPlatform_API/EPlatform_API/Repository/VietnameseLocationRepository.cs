using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.IRepository;
using EPlatform_API.Models.Regions;
using Microsoft.EntityFrameworkCore;

namespace EPlatform_API.Repository
{
    public class VietnameseLocationRepository : IVietnameseLocationRepository
    {
        private readonly VietnameseLocationContext _context;

        public VietnameseLocationRepository(
            VietnameseLocationContext context
        )
        {
            _context = context;
        }

        public async Task<IList<District>> GetDistrictsByProvinceAsync(string provinceId)
        {
            var districts = await _context.Districts.AsNoTracking().Where(d => d.ProvinceCode == provinceId).ToListAsync();
            return districts;
        }

        public async Task<IList<Province>> GetProvincesAsync()
        {
            var provinces = await _context.Provinces.AsNoTracking().ToListAsync();
            return provinces;
        }

        public async Task<IList<Ward>> GetWardsByDistrictAsync(string districtId)
        {
            var wards = await _context.Wards.AsNoTracking().Where(w => w.DistrictCode == districtId).ToListAsync();
            return wards;
        }
    }
}
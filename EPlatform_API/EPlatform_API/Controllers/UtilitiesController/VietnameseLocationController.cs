using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.IRepository;
using EPlatform_API.Models.Regions;
using Microsoft.AspNetCore.Mvc;

namespace EPlatform_API.Controllers.UtilitiesController
{
    [Route("api/v1/vietnameese-location")]
    [ApiController]
    public class VietnameseLocationController : ControllerBase
    {
        private readonly IVietnameseLocationRepository _vietnameseLocationRepo;
        public VietnameseLocationController(
            IVietnameseLocationRepository vietnameseLocationRepository
        )
        {
            _vietnameseLocationRepo = vietnameseLocationRepository;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _vietnameseLocationRepo.GetProvincesAsync();
            return Ok(new ApiResponseStandard<IList<Province>>{
                Status = 200,
                Message = "Success",
                Data = provinces,
            });
        }

        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistrictsByProvince(string provinceId)
        {
            var districts = await _vietnameseLocationRepo.GetDistrictsByProvinceAsync(provinceId);
            return Ok(new ApiResponseStandard<IList<District>>{
                Status = 200,
                Message = "Success",
                Data = districts,
            });
        }

        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWardsByDistrict(string districtId)
        {
            var wards = await _vietnameseLocationRepo.GetWardsByDistrictAsync(districtId);
            return Ok(new ApiResponseStandard<IList<Ward>>{
                Status = 200,
                Message = "Success",
                Data = wards,
            });
        }

    }
}
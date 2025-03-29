using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace EPlatform_API.Controllers.UtilitiesController
{
    [ApiController]
    [Route("api/v1/utilities")]
    public class UtilitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UtilitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ware-houses")]
        public async Task<IActionResult> GetWareHouses()
        {
            var warehouses = _context.Warehouses.ToList();
            return Ok(warehouses);
        }

        [HttpGet("ware-houses-for-select")]
        public async Task<IActionResult> GetWareHousesForSelect()
        {
            var warehouses = _context.Warehouses
            .Select(wh => new {
                id = wh.WarehouseId,
                name = wh.Name
            })
            .ToList();
            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Warehouses found",
                Data = warehouses
            });
        }
    
        [HttpDelete("categories-clear")]
        public async Task<IActionResult> ClearCategories()
        {
            _context.Categories.RemoveRange(_context.Categories);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Categories cleared"
            });
        }

    }
}
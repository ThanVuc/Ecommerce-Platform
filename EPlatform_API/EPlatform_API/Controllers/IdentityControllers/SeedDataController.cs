using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.UtilitiesDTOs;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EPlatform_API.Controllers.IdentityControllers
{
    [ApiController]
    [Route("api/v1/seed")]
    public class SeedDataController : ControllerBase
    {
        private readonly ISeedDataService _seedDataService;
        private readonly IMongoRepository<Chat> _mongoRepository;
        private readonly IRepositoryBase<AppUser> _userRepo;
        private readonly ShopRepository _shopRepo;
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;


        public SeedDataController(
            ISeedDataService seedDataService,
            IMongoRepository<Chat> mongoRepository,
            IRepositoryBase<AppUser> userRepository,
            AppDbContext context,
            IUnitOfWork unitOfWork
        )
        {
            _seedDataService = seedDataService;
            _mongoRepository = mongoRepository;
            _userRepo = userRepository;
            _context = context;
            _shopRepo = unitOfWork.ShopRepo;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> SeedRole(){
            await _seedDataService.SeedRoleData();
            return Ok("Seed Data Successful!");
        }

        [HttpGet("users")]
        public async Task<IActionResult> SeedUser(){
            await _seedDataService.SeedUserData();
            return Ok("Seed Data Successful!");
        }

        [HttpGet("shops")]
        public async Task<IActionResult> SeedShop(){
            await _seedDataService.SeedShopData();
            return Ok("Seed Data Successful!");
        }

        [HttpGet("seed-categories-data")]
        public async Task<IActionResult> SeedCategoriesData(){
            await _seedDataService.SeedCategoryData();
            return Ok("Seed Data Successful!");
        }

        [HttpGet("seed-vietnamese-location")]
        public async Task<IActionResult> SeedVietnameseLocationData(){
            await _seedDataService.SeedVietnameseLocationData();
            return Ok("Seed Data Successful!");
        }

        [HttpPost("seed-warehouse")]
        public async Task<IActionResult> AddWarehouse( [FromBody] AddWarehouseRequest warehouseRequest){
            if (warehouseRequest == null){
                return StatusCode(400, new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "Invalid request"
                });
            }

            _context.Warehouses.Add(new WareHouse{
                Name = warehouseRequest.Name,
                Capability = warehouseRequest.Capability,
                Phone = warehouseRequest.Phone,
                Email = warehouseRequest.Email,
                Location = warehouseRequest.Location
            });

            await _context.SaveChangesAsync();

            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Warehouse added",
                Data = warehouseRequest
            });
        }
    }
}
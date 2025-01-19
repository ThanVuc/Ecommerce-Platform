using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.ShopDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.Mappers;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace EPlatform_API.Controllers.ShopOwnerControllers
{
    [ApiController]
    [Route("api/v1/shops")]
    public class ShopController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ShopRepository _shopRepo;
        private readonly IDatabase _redisDb;

        public ShopController(
            IUnitOfWork unitOfWork
        )
        {
            _unitOfWork = unitOfWork;
            _shopRepo = _unitOfWork.ShopRepo;
            _redisDb = RedisManager.Connection.GetDatabase();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShop(){
            var shops = await _shopRepo.GetAllAsync();
            return Ok(shops);
        }
        
        [HttpGet("layout/{shopId}")]
        public async Task<IActionResult> GetShopByIdLayout(string shopId){
            var shopLayout = await _shopRepo.GetShopByIdlayoutAsync(shopId);

            if(shopLayout == null){
                return StatusCode(404,new ApiResponseStandard<object>{
                    Status = 404,
                    Message = "Shop not found"
                });
            }

            return Ok(new ApiResponseStandard<ShopLayoutResponse>{
                Status = 200,
                Message = "Shop found",
                Data = shopLayout
            });
        }

        [HttpGet("{shopId}")]
        public async Task<IActionResult> GetShopById(string shopId){
            var shop = await _shopRepo.GetShopResponseByIdAsync(shopId);

            if(shop == null){
                return StatusCode(404,new ApiResponseStandard<object>{
                    Status = 404,
                    Message = "Shop not found"
                });
            }

            return Ok(new ApiResponseStandard<ShopDetailResponse>{
                Status = 200,
                Message = "Shop found",
                Data = shop
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopRequest shopRequest){
            if (shopRequest == null){
                return StatusCode(400, new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "Invalid request"
                });
            }
            
            var shop = shopRequest.ToShop();
            await _shopRepo.AddAsync(shop);
            await _unitOfWork.SaveAsync();

            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Shop created",
                Data = shop
            });
        }
    
        [HttpPut("{shopId}/update")]
        public async Task<IActionResult> UpdateShop(string shopId, [FromBody] UpdateShopRequest shopRequest){
            if (shopRequest == null){
                return StatusCode(400, new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "Invalid request"
                });
            }

            var shop = await _shopRepo.GetShopByIdAsync(shopId);
            if(shop == null){
                return StatusCode(404, new ApiResponseStandard<object>{
                    Status = 404,
                    Message = "Shop not found"
                });
            }

            shop.Name = shopRequest.Name ?? shop.Name;
            shop.PickUpAddress = shopRequest.PickUpAddress ?? shop.PickUpAddress;
            shop.Email = shopRequest.Email ?? shop.Email;
            shop.Phone = shopRequest.Phone ?? shop.Phone;
            shop.ShopAddress = shopRequest.ShopAddress ?? shop.ShopAddress;
            shop.InvoiceEmail = shopRequest.InvoiceEmail ?? shop.InvoiceEmail;
            shop.TaxesCode = shopRequest.TaxesCode ?? shop.TaxesCode;
            shop.IdentificationNumber = shopRequest.IdentificationNumber ?? shop.IdentificationNumber;

            _shopRepo.Update(shop);
            await _unitOfWork.SaveAsync();

            _redisDb.KeyDelete($"shop:{shopId}");
            _redisDb.KeyDelete($"shop-layout:{shopId}");

            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Shop updated",
                Data = shop
            });
        }
    
        [HttpDelete("{shopId}/delete")]
        public async Task<IActionResult> DeleteShop(string shopId){
            var shop = await _shopRepo.GetShopByIdAsync(shopId);
            if(shop == null){
                return StatusCode(404, new ApiResponseStandard<object>{
                    Status = 404,
                    Message = "Shop not found"
                });
            }

            _shopRepo.Delete(shop);
            await _unitOfWork.SaveAsync();

            _redisDb.KeyDelete($"shop:{shopId}");
            _redisDb.KeyDelete($"shop-layout:{shopId}");

            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Shop deleted"
            });
        }
    }
}
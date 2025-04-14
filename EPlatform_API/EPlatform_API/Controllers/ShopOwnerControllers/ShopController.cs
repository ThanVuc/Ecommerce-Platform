using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.ShopDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Mappers;
using EPlatform_API.Models;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;
using EPlatform_API.Services;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using StackExchange.Redis;

namespace EPlatform_API.Controllers.ShopOwnerControllers
{
    [ApiController]
    [Route("api/v1/shops")]
    [Authorize]
    public class ShopController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ShopRepository _shopRepo;
        private readonly IBlobServices _blogService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDatabase _redisDb;
        private readonly NotificationRepo _notificationRepo;

        public ShopController(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            NotificationRepo notificationRepo,
            IConnectionMultiplexer redis
        )
        {
            _unitOfWork = unitOfWork;
            _shopRepo = _unitOfWork.ShopRepo;
            _redisDb = redis.GetDatabase();
            _blogService = new BlobServices(configuration, BlobStorage.PublicImages);
            _userManager = userManager;
            _notificationRepo = notificationRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShop()
        {
            var shops = await _shopRepo.GetAllAsync();
            return Ok(shops);
        }

        [HttpGet("layout/{shopId}")]
        public async Task<IActionResult> GetShopByIdLayout(string shopId)
        {
            if (string.IsNullOrEmpty(shopId))
            {
                return StatusCode(400, new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "ShopId is required"
                });
            }

            if (shopId != GetUserIdFromToken())
            {
                return Unauthorized(new ApiResponseStandard<object>
                {
                    Status = 401,
                    Message = "Unauthorized"
                });
            }
            var shopLayout = await _shopRepo.GetShopByIdlayoutAsync(shopId);

            if (shopLayout == null)
            {
                return StatusCode(404, new ApiResponseStandard<object>
                {
                    Status = 404,
                    Message = "Shop not found"
                });
            }

            return Ok(new ApiResponseStandard<ShopLayoutResponse>
            {
                Status = 200,
                Message = "Shop found",
                Data = shopLayout
            });
        }

        [HttpGet("{shopId}")]
        public async Task<IActionResult> GetShopById(string shopId)
        {
            if (string.IsNullOrEmpty(shopId))
            {
                return StatusCode(400, new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "ShopId is required"
                });
            }

            if (shopId != GetUserIdFromToken())
            {
                return Unauthorized(new ApiResponseStandard<object>
                {
                    Status = 401,
                    Message = "Unauthorized"
                });
            }
            var shop = await _shopRepo.GetShopResponseByIdAsync(shopId);

            if (shop == null)
            {
                return StatusCode(404, new ApiResponseStandard<object>
                {
                    Status = 404,
                    Message = "Shop not found"
                });
            }

            return Ok(new ApiResponseStandard<ShopDetailResponse>
            {
                Status = 200,
                Message = "Shop found",
                Data = shop
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateShop([FromForm] CreateShopRequest createShopModel)
        {
            if (createShopModel == null)
            {
                return StatusCode(400, new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Invalid request"
                });
            }

            var shop = createShopModel.ToShop();

            // delete email by retrive the name in url
            if (createShopModel.LogoImage != null)
            {
                var fileName = $"{UtilityServices.GenerateRandomString(5)}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                shop.LogoUrl = await _blogService.UploadImageAsync(
                    _blogService.ConvertToFileStreamModel(fileName, createShopModel.LogoImage)
                );
            }
            else
            {
                return StatusCode(400, new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Invalid request due to missing logo image"
                });
            }

            try
            {
                await _unitOfWork.BeginTransaction();

                if (createShopModel.ShopId == null)
                {
                    await _unitOfWork.RollBackTransaction();
                    throw new Exception("ShopId is required");
                }

                var user = await _userManager.FindByIdAsync(createShopModel.ShopId);

                if (user == null)
                {
                    await _unitOfWork.RollBackTransaction();
                    throw new Exception("User not found");
                }

                if (await _shopRepo.IsExist(user.Id)){
                    await _unitOfWork.RollBackTransaction();
                    throw new Exception("Shop already exist");
                }

                var addRoleRs = await _userManager.AddToRoleAsync(user, RoleStorage.ShopOwner);

                if (!addRoleRs.Succeeded)
                {
                    await _unitOfWork.RollBackTransaction();
                    throw new Exception("Failed to add role");
                }

                await _shopRepo.CreateShopAsync(shop);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransaction();

                return Ok(new ApiResponseStandard<Shop>
                {
                    Status = 200,
                    Message = "Shop created",
                    Data = shop
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = e.Message
                });
            }
        }

        [HttpGet("get-user-id")]
        public async Task<IActionResult> GetUserId()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Ok(new ApiResponseStandard<object>
                {
                    Status = 200,
                    Message = "User found",
                    Data = userId
                });
            }
            catch (NullReferenceException e)
            {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = e.Message
                });
            }
        }
    
        [HttpGet("{shopId}/get-notifications")]
        public async Task<IActionResult> GetNotification(string shopId)
        {
            if (string.IsNullOrEmpty(shopId))
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "ShopId is required"
                });
            }

            if (shopId != GetUserIdFromToken())
            {
                return Unauthorized(new ApiResponseStandard<object>
                {
                    Status = 401,
                    Message = "Unauthorized"
                });
            }
            
            var notifications = await _notificationRepo.GetNotificationByShopIdAsync(shopId);
            if (notifications == null)
            {
                return StatusCode(404, new ApiResponseStandard<object>
                {
                    Status = 404,
                    Message = "Notifications not found"
                });
            }

            return Ok(new ApiResponseStandard<List<ShopNotification>>
            {
                Status = 200,
                Message = "Notifications found",
                Data = notifications
            });
        }

        [HttpDelete("{shopId}/notifications/{notificationId}/remove")]
        public async Task<IActionResult> RemoveNotification(string shopId, string notificationId)
        {
            if (string.IsNullOrEmpty(shopId) || string.IsNullOrEmpty(notificationId))
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "ShopId and NotificationId are required"
                });
            }

            if (shopId != GetUserIdFromToken())
            {
                return Unauthorized(new ApiResponseStandard<object>
                {
                    Status = 401,
                    Message = "Unauthorized"
                });
            }

            await _notificationRepo.RemoveNotificationAsync(notificationId);
            await _unitOfWork.SaveAsync();

            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Notification removed successfully"
            });
        }

        private string GetUserIdFromToken(){
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User not found");
            }
            return userId;
        }
    }
}
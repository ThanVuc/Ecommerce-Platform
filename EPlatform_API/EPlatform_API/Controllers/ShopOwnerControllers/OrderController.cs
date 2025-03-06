using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.ProductDTOs;
using EPlatform_API.IServices;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPlatform_API.Controllers.ShopOwnerControllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductController> _logger;
        private readonly ILoggingService _loggingService;
        public OrderController(
            IUnitOfWork unitOfWork,
            ILogger<ProductController> logger,
            ILoggingService loggingService
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _loggingService = loggingService;
        }

        [HttpPost("create-order")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrdersRequest request)
        {
            var customerName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (customerName == null)
            {
                return Unauthorized(new ApiResponseStandard<object>
                {
                    Message = "Unauthorized",
                    Data = "You must login to create order"
                });
            }

            // var order = await _productRepo.CreateOrder(customerName, request);
            // await _unitOfWork.SaveAsync();
            return Ok(new ApiResponseStandard<object>
            {
                Status = 201,
                Message = "Create order",
                Data = request
            });
        }
    }
}
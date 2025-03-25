using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.OrderDTOs;
using EPlatform_API.DTOs.ProductDTOs;
using EPlatform_API.Helper;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Mappers;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;
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
        private readonly UserRepo _userRepo;
        private readonly ProductRepository _productRepo;
        private readonly OrderRepository _orderRepository;
        private readonly ShopRepository _shopRepository;
        private readonly ProductInfoMongoRepository _productInfoMongoRepository;

        public OrderController(
            IUnitOfWork unitOfWork,
            ILogger<ProductController> logger,
            ILoggingService loggingService,
            UserRepo userRepo,
            OrderRepository orderRepository,
            ShopRepository shopRepository,
            ProductInfoMongoRepository productInfoMongoRepository
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _loggingService = loggingService;
            _userRepo = userRepo;
            _productRepo = _unitOfWork.ProductRepo;
            _orderRepository = orderRepository;
            _shopRepository = shopRepository;
            _productInfoMongoRepository = productInfoMongoRepository;
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

            var userId = await _userRepo.GetUserIdByEmail(customerName);
            if (userId == null)
            {
                return NotFound(new ApiResponseStandard<object>
                {
                    Status = 404,
                    Message = "Not Found",
                    Data = "User not found"
                });
            }
            // products of shop
            var shopProductDict = new Dictionary<string, List<CartItemOfOrder>>();

            if (request.CartItems == null || request.CartItems.Count == 0)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Bad Request",
                    Data = "Cart items is empty"
                });
            }

            try {
                foreach (var cartItem in request.CartItems)
                {
                    if (shopProductDict.ContainsKey(cartItem.ShopId))
                    {
                        shopProductDict[cartItem.ShopId].Add(cartItem);
                    }
                    else
                    {
                        if (!await _shopRepository.IsExist(cartItem.ShopId)){
                            return NotFound(new ApiResponseStandard<object>
                            {
                                Status = 404,
                                Message = "Not Found",
                                Data = $"Shop with id {cartItem.ShopId} not found"
                            });
                        }

                        shopProductDict.Add(cartItem.ShopId, new List<CartItemOfOrder> {cartItem});
                    }
                }
            } catch (Exception e) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = $"Error when create order in order controller, {e.Message}"
                });
            }
            
            await _unitOfWork.BeginTransaction();

            try {
                await _productInfoMongoRepository.MinusInventory(request.CartItems);
            } catch (Exception e) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = $"Error when create order in order controller, {e.Message}"
                });
            }

            // Minus quantity of product
            try {
                await _productRepo.MinusProductInventory(request.CartItems);
                await _unitOfWork.SaveAsync();
            } catch (Exception e) {
                await _unitOfWork.RollBackTransaction();
                await _productInfoMongoRepository.RollBackInventory(request.CartItems);
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = $"Error when create order in order controller, {e.Message}"
                });
            }

            // Create order
            try {
                foreach (var shopProduct in shopProductDict)
                {
                    await _orderRepository.CreateOrder(new CreateOrdersRequest{ Email = request.Email, Phone = request.Phone, ShippingAddress = request.ShippingAddress } ,shopProduct, userId);
                }
            } catch (Exception e) {
                await _unitOfWork.RollBackTransaction();
                await _productInfoMongoRepository.RollBackInventory(request.CartItems);
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = $"Error when create order in order controller, {e.Message}"
                });
            }

            // clear cart
            try {
                await _productRepo.ClearCart(request.CartItems);
            } catch (Exception e) {
                await _unitOfWork.RollBackTransaction();
                await _productInfoMongoRepository.RollBackInventory(request.CartItems);
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = $"Error when create order in order controller, {e.Message}"
                });
            }

            await _unitOfWork.CommitTransaction();
            await _unitOfWork.SaveAsync();

            return Ok(new ApiResponseStandard<object>
            {
                Status = 201,
                Message = "Create order successfully",
                Data = request
            });
        }
    
        [HttpGet("/api/v1/shops/{shopId}/orders")]
        [Authorize(Roles = "ShopOwner")]
        public async Task<IActionResult> GetOrdersByShop([FromRoute] string shopId, [FromQuery] OrderManagementQueryString queryString)
        {
            if (string.IsNullOrEmpty(shopId))
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Bad Request",
                    Data = "Shop id is required"
                });
            }

            try {
                // 1: get all orders by shop, filter by order status id if needed
                //  search by order id or product name if needed
                // return type is IQueryable so it not execute query to database
                var orders = await _orderRepository.GetOrderByShop(shopId, queryString.OrderStatusId, queryString.SearchString);

                // 2: paging orders and convert to GetAllOrderResponse
                var ordersPageList = PageList<GetAllOrderResponse>.ToPageList(orders.Select(o => o.ToGetAllOrderResponse()).AsQueryable(), queryString.PageNumber, queryString.PageSize);
                ordersPageList.AddPagingInfoToHeader(Response);

                // 3: get orders with needed fields
                return Ok(new ApiResponseStandard<object>
                {
                    Status = 200,
                    Message = "Get orders by shop successfully",
                    Data = ordersPageList
                });
            } catch (Exception ex) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Error",
                    Data = ex.Message
                });
            }
        }
    
        [HttpGet("get-all-status")]
        [Authorize]
        public IActionResult GetAllOrderStatus()
        {
            var orderStatuses = _orderRepository.GetAllOrderStatus();
            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Get all order status successfully",
                Data = orderStatuses
            });
        }
    }
}
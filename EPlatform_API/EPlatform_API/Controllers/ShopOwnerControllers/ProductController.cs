using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.ProductDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.Helper;
using EPlatform_API.IServices;
using EPlatform_API.Repository;
using EPlatform_API.Services;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace EPlatform_API.Controllers.ShopOwnerControllers
{
    [Route("api/v1/product-items")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductRepository _productRepo;
        private readonly IBlobServices _imagesBlobServices;
        private readonly ILogger<ProductController> _logger;
        private readonly ProductInfoMongoRepository _productInfoMongoRepo;
        private readonly ILoggingService _loggingService;
        private readonly SearchMongoRepo _searchMongoRepo;

        public ProductController(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<ProductController> logger,
            ILoggingService loggingService,
            ProductInfoMongoRepository productInfoMongoRepository,
            SearchMongoRepo searchMongoRepo
        )
        {
            _unitOfWork = unitOfWork;
            _productRepo = unitOfWork.ProductRepo;
            _imagesBlobServices = new BlobServices(configuration, BlobStorage.PublicImages);
            _logger = logger;
            _loggingService = loggingService;
            _productInfoMongoRepo = productInfoMongoRepository;
            _searchMongoRepo = searchMongoRepo;
        }

        [HttpGet("/api/v1/categories-in-home")]
        public async Task<IActionResult> GetCategoriesInHome()
        {
            var categories = await _productRepo.GetCategoriesInHome();
            return Ok(new ApiResponseStandard<object>
            {
                Message = "Categories in home",
                Data = categories
            });
        }

        [HttpGet("hot-products")]
        public async Task<IActionResult> GetHotProducts()
        {
            var products = await _productRepo.GetHotProducts();
            return Ok(new ApiResponseStandard<object>
            {
                Message = "Products",
                Data = products
            });
        }

        [HttpGet("today-suggestions")]
        public async Task<IActionResult> GetTodaySuggestions()
        {
            var products = await _productRepo.GetTodaySuggestions();
            return Ok(new ApiResponseStandard<object>
            {
                Message = "Products",
                Data = products
            });
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                var product = await _productRepo.GetProductById(productId);
                var productSpecInfo = await _productInfoMongoRepo.GetProductSpecInfoByProductIdAsync(productId);

                var productResponse = new
                {
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    AvtImageUrl = product.AvtImgUrl,
                    Sold = UtilityServices.ConvertBigNumberToShortNumber((long)product.Inventory.SoldQuantity),
                    Availabel = product.Inventory.AvailableQuantity,
                    SpecAttributes = productSpecInfo.SpecInfos.Select(s => new
                    {
                        SpecName = s.SpecName,
                        IsPrimary = s.IsPrimary,
                        SpecItems = s.SpecItems?.Select(si => new
                        {
                            SpecValue = si.SpecValue,
                            SpecImageUrl = si.SpecImageUrl
                        }).ToList()
                    }).ToList(),
                    SpecInventories = productSpecInfo.SpecInfoInventories.Select(si => new
                    {
                        PrimarySpecValueName = si.PrimarySpecValueName,
                        SubSpecValueName = si.SubSpecValueName,
                        Inventory = si.Inventory
                    }).ToList(),
                    Categories = await GetAllParentCategories(productId),
                    ShopId = product.ShopId,
                    ShopName = product.Shop.Name,
                    LogoUrl = product.Shop.LogoUrl,
                };

                return Ok(new ApiResponseStandard<object>
                {
                    Status = 200,
                    Message = "Product",
                    Data = productResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Message = "Error",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("/api/v1/carts/add-to-cart")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddItemToCart request)
        {
            var customerName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (customerName == null)
            {
                return Unauthorized(new ApiResponseStandard<object>
                {
                    Message = "Unauthorized",
                    Data = "You must login to add item to cart"
                });
            }
            if (string.IsNullOrEmpty(request.SpecInfo)){
                return BadRequest(new ApiResponseStandard<object>
                {
                    Message = "Bad request",
                    Data = "Spec info is required"
                });
            }
            var cartItem = await _productRepo.AddItemToCart(customerName ,request);
            await _unitOfWork.SaveAsync();
            return Ok(new ApiResponseStandard<object>
            {
                Message = "Add item to cart",
                Data = cartItem.CartItemId
            });
        }

        [HttpGet("/api/v1/carts")]
        [Authorize]
        public async Task<IActionResult> GetCartItems()
        {
            var customerName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (customerName == null){
                return Unauthorized(new ApiResponseStandard<object>
                {
                    Message = "Unauthorized",
                    Data = "You must login to get cart items"
                });
            }

            var carts = await _productRepo.GetCartItems(customerName);
            foreach (var item in carts)
            {
                var specInfo = item.SpecInfo;
                if (specInfo != null)
                {
                    var both = specInfo.Split(':');
                    if (both.Length == 2)
                    {
                        var specValue = both[1].Trim();
                        item.AvailableQuantity = await _productInfoMongoRepo.GetAvailableInventory(item.ProductId, specValue);
                    } else {
                        // mix value is wrong with the expectation
                        var mix = both[1].Split(',');
                        var primary = mix[0].Trim();
                        var sub = both.Last().Trim();
                        item.AvailableQuantity = await _productInfoMongoRepo.GetAvailableInventory(item.ProductId, primary, sub);
                    }
                }
            }
            return Ok(new ApiResponseStandard<object>
            {
                Message = "Cart items",
                Data = carts
            });
        }

        [HttpDelete("/api/v1/carts/{cartId}/remove-item")]
        public async Task<IActionResult> RemoveProductFromCart(int cartId){
            try {
                await _productRepo.RemoveCartItems(cartId);
                await _unitOfWork.SaveAsync();
            } catch (Exception ex) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Message = "Error",
                    Data = ex.Message
                });
            }

            return Ok(new ApiResponseStandard<object>
            {
                Message = "Remove item from cart",
                Data = "remove successful"
            });
        }
        
        [HttpGet("/api/v1/carts/get-cart-number")]
        [Authorize]
        public async Task<IActionResult> GetCartNumber()
        {
            var customerName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (customerName == null){
                return Unauthorized(new ApiResponseStandard<object>
                {
                    Message = "Unauthorized",
                    Data = "You must login to get cart items"
                });
            }

            var cartCount = await _productRepo.GetCartItemsCount(customerName);
            return Ok(new ApiResponseStandard<object>
            {
                Message = "Cart items",
                Data = cartCount
            });
        }

        private async Task<object> GetAllParentCategories(int productId)
        {
            var product = await _productRepo.GetProductById(productId);
            var categories = product.Category.getAllParentCategories()
            .Select(c => new
            {
                c.CategoryId,
                c.Name,
                c.Slug
            }).ToList();
            return categories;
        }

        [HttpGet("/api/v1/products/search")]
        public async Task<IActionResult> SearchProducts([FromQuery] SearchProductQueryDTO query)
        {
            try {
                // search products by name and select by category id
                var products = await _productRepo.SearchProducts(query.SearchString, query.CategoryId);
                if (products == null || products.Count() == 0) {
                    return Ok(new ApiResponseStandard<object>
                    {
                        Message = "Products",
                        Data = products
                    });
                }
                // pagination
                var productsRes = PageList<SearchProductsReponse>.ToPageList(products, query.PageNumber, query.PageSize);
                productsRes.AddPagingInfoToHeader(Response);
                // increase search frequences
                // contition quite redundant, but for the sake of clarity, we keep it here
                if (query.SearchString != null && query.SearchString.Length > 0 && query.CategoryId == 0) {
                    await IncreaseSearchFrequences(query.SearchString);   
                }

                return Ok(new ApiResponseStandard<object>
                {
                    Message = "Products",
                    Data = productsRes
                });
            } catch (Exception ex) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Message = "Error",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("/api/v1/products/autocomplete")]
        public async Task<IActionResult> GetAutoCompleteProducts([FromQuery] string prefix)
        {
            try {
                var products = await _searchMongoRepo.GetAllProductsForAutocomplete(prefix);
                return Ok(new ApiResponseStandard<object>
                {
                    Message = "Products",
                    Data = products.Select(p => new
                    {
                        p.Name
                    }).ToList()
                });
            } catch (Exception ex) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Message = "Error",
                    Data = ex.Message
                });
            }
        }

        private async Task IncreaseSearchFrequences(string productName)
        {
            try {
                var result = await _searchMongoRepo.InsertOrUpdateSearchProductAnalysic(productName);
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Repository;
using EPlatform_API.Services;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

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
        
        public ProductController(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<ProductController> logger,
            ILoggingService loggingService,
            ProductInfoMongoRepository productInfoMongoRepository
        )
        {
            _unitOfWork = unitOfWork;
            _productRepo = unitOfWork.ProductRepo;
            _imagesBlobServices = new BlobServices(configuration, BlobStorage.PublicImages);
            _logger = logger;
            _loggingService = loggingService;
            _productInfoMongoRepo = productInfoMongoRepository;
        }

        [HttpGet("/api/v1/categories-in-home")]
        public async Task<IActionResult> GetCategoriesInHome()
        {
            var categories = await _productRepo.GetCategoriesInHome();
            return Ok(new ApiResponseStandard<object>{
                Message = "Categories in home",
                Data = categories
            });
        }

        [HttpGet("hot-products")]
        public async Task<IActionResult> GetHotProducts()
        {
            var products = await _productRepo.GetHotProducts();
            return Ok(new ApiResponseStandard<object>{
                Message = "Products",
                Data = products
            });
        }

        [HttpGet("today-suggestions")]
        public async Task<IActionResult> GetTodaySuggestions()
        {
            var products = await _productRepo.GetTodaySuggestions();
            return Ok(new ApiResponseStandard<object>{
                Message = "Products",
                Data = products
            });
        }
    }
}
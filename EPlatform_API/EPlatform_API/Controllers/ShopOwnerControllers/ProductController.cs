using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus.DataSets;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.AuthDTOs.Users;
using EPlatform_API.DTOs.FileDTOs;
using EPlatform_API.DTOs.ProductDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.Helper;
using EPlatform_API.IRepository;
using EPlatform_API.IServices;
using EPlatform_API.Mappers;
using EPlatform_API.Models;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;
using EPlatform_API.Services;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace EPlatform_API.Controllers.ShopOwnerControllers
{
    [ApiController]
    [Route("api/v1/shops/{shopId}/products")]
    [Authorize(Roles = "ShopOwner")]
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

        [HttpGet]
        public async Task<IActionResult> GetProductByShopId([FromRoute] string shopId, [FromQuery] UserQueryStringModel queryStringModel)
        {
            if (shopId == null)
            {
                return StatusCode(400, new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "The shop is empty"
                });
            }



            var productQueryable = _productRepo.GetProductsByShopSummerize(shopId);

            if (queryStringModel.SearchString != null)
            {
                productQueryable = productQueryable
                .Where(p => p.Name.Contains(queryStringModel.SearchString));
            }

            var products = PageList<Product>.ToPageList(productQueryable.AsQueryable(), queryStringModel.PageNumber, queryStringModel.PageSize);
            products.AddPagingInfoToHeader(Response);

            var apiRes = products.Select(p => new
            {
                ProductId = p.ProductId,
                AvtImgUrl = p.AvtImgUrl,
                Price = p.Price,
                IsPublic = p.IsPublic,
                Name = p.Name,
                Inventory = p.Inventory,
                Slug = p.Slug
            });

            return Ok(new ApiResponseStandard<object>
            {
                Message = "products list api",
                Status = 200,
                Timestamp = DateTime.Now,
                Data = apiRes
            });
        }

        [HttpPut("public-or-hide-product")]
        public async Task<IActionResult> PublicOrHideProduct([FromBody] PublicOrHideProductRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "The request body is empty"
                });
            }

            var product = await _productRepo.GetProductByIdAsync(request.ProductId);


            if (product == null)
            {
                return NotFound(new ApiResponseStandard<object>
                {
                    Status = 404,
                    Message = "Product not found"
                });
            }

            product.IsPublic = request.IsPublic;
            _productRepo.Update(product);
            await _unitOfWork.SaveAsync();

            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Update product success"
            });
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromRoute] string shopId, [FromForm] AddProductRequest addProductRequest)
        {
            if (addProductRequest == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "The request body is empty"
                });
            }

            if (shopId == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "The shop is empty"
                });
            }

            // upload image
            var imagesNameDict = new Dictionary<string, ImageStoreModel>();
            try {
                imagesNameDict = await UploadProductImageAsync(addProductRequest);
            } catch (System.NullReferenceException ex){
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "The specialize image of each field is require"
                });
            }

            // add product core to sql server
            var product = addProductRequest.ToProduct();
            product.ShopId = shopId;
            product.AvtImgUrl = imagesNameDict["coverImage"].Url;
            product.AvtImgName = imagesNameDict["coverImage"].Name;
            product.Inventory = new Inventory
            {
                Quantity = addProductRequest.TotalInventory,
                IsAvailable = true,
                AvailableQuantity = addProductRequest.TotalInventory,
                UpdatedAt = DateTime.Now,
                SoldQuantity = 0,
                WareHouseId = addProductRequest.WarehouseId,
            };
            await _productRepo.AddProductAsync(product);
            await _unitOfWork.SaveAsync();

            // save product info spec to mongodb
            var productSpecInfo = addProductRequest.ToProductSpecInfo(product.ProductId, imagesNameDict);

            await _productInfoMongoRepo.CreateAsync(productSpecInfo);

            // save all file to sql db
            await _unitOfWork.SaveAsync();

            return Ok(productSpecInfo);
        }

        [HttpGet("/api/v1/categories")]
        public async Task<IActionResult> GetCategories([FromQuery] int? parentCategoryId = null, [FromQuery] string? searchString = null)
        {
            var categories = await _productRepo.GetCategoriesAsync(parentCategoryId, searchString);
            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Get categories success",
                Data = categories
            });
        }

        [HttpGet("/api/v1/products/{productId}/update")]
        public async Task<IActionResult> GetProductUpdateById([FromRoute] int productId)
        {
            var productResponse = new UpdateProductResponse();
            try
            {
                var product = await _productRepo.GetProductAllByIdAsync(productId);
                var productSpecInfo = await _productInfoMongoRepo.GetProductSpecInfoByProductIdAsync(productId);

                if (product == null || productSpecInfo == null)
                {
                    return NotFound(new ApiResponseStandard<object>
                    {
                        Status = 404,
                        Message = "Product not found"
                    });
                }

                productResponse = new UpdateProductResponse
                {
                    Name = product.Name,
                    CategoryId = product.CategoryId,
                    CategoryName = product?.Category?.Name == null ? "" : product.Category.Name,
                    Description = product.Description,
                    Slug = product.Slug,
                    Price = product.Price == null ? 0 : (decimal)product.Price,
                    IsPublic = product.IsPublic,
                    SpecAttributes = productSpecInfo.SpecInfos.Select(s => new SpecAttributeUpdate
                    {
                        SpecName = s.SpecName,
                        IsPrimary = s.IsPrimary,
                        SpecItems = s.SpecItems?.Select(si => new SpecItemUpdate
                        {
                            SpecValue = si.SpecValue,
                            SpecImageUrl = si.SpecImageUrl
                        }).ToList()
                    }).ToList(),
                    SpecInventories = productSpecInfo.SpecInfoInventories.Select(si => new SpecInventoryUpdate
                    {
                        PrimarySpecValueName = si.PrimarySpecValueName,
                        SubSpecValueName = si.SubSpecValueName,
                        Inventory = si.Inventory
                    }).ToList(),
                    WarehouseId = product.Inventory.WareHouseId,
                    TotalInventory = product.Inventory == null ? 0 : (int)product.Inventory.Quantity,
                    CoverImageUrl = product.AvtImgUrl
                };

                return Ok(new ApiResponseStandard<object>
                {
                    Status = 200,
                    Message = "Get product success",
                    Data = productResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("/api/v1/products/{productId}")]
        public async Task<IActionResult> GetProductDetailById([FromRoute] int productId)
        {
            var productResponse = new ProductDetailResponse();
            try
            {
                var product = await _productRepo.GetProductAllByIdAsync(productId);
                var productSpecInfo = await _productInfoMongoRepo.GetProductSpecInfoByProductIdAsync(productId);

                if (product == null || productSpecInfo == null)
                {
                    return NotFound(new ApiResponseStandard<object>
                    {
                        Status = 404,
                        Message = "Product not found"
                    });
                }

                productResponse = new ProductDetailResponse
                {
                    Name = product.Name,
                    CategoryId = product.CategoryId,
                    CategoryName = product?.Category?.Name == null ? "" : product.Category.Name,
                    Description = product.Description,
                    Slug = product.Slug,
                    Price = product.Price == null ? 0 : (decimal)product.Price,
                    IsPublic = product.IsPublic,
                    SpecAttributes = productSpecInfo.SpecInfos.Select(s => new SpecAttributeUpdate
                    {
                        SpecName = s.SpecName,
                        IsPrimary = s.IsPrimary,
                        SpecItems = s.SpecItems?.Select(si => new SpecItemUpdate
                        {
                            SpecValue = si.SpecValue,
                            SpecImageUrl = si.SpecImageUrl
                        }).ToList()
                    }).ToList(),
                    SpecInventories = productSpecInfo.SpecInfoInventories.Select(si => new SpecInventoryUpdate
                    {
                        PrimarySpecValueName = si.PrimarySpecValueName,
                        SubSpecValueName = si.SubSpecValueName,
                        Inventory = si.Inventory
                    }).ToList(),
                    WarehouseId = product.Inventory.WareHouseId,
                    TotalInventory = product.Inventory == null ? 0 : (int)product.Inventory.Quantity,
                    CoverImageUrl = product.AvtImgUrl,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return Ok(new ApiResponseStandard<object>
                {
                    Status = 200,
                    Message = "Get product success",
                    Data = productResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = ex.Message
                });
            }
        }


        [HttpPut("/api/v1/products/{productId}/update")]
        public async Task<IActionResult> UpdateProductById([FromRoute] int productId, [FromForm] AddProductRequest updateProductModel)
        {
            if (updateProductModel == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "The request body is empty"
                });
            }

            var imgDict = await UpdateProductImageAsync(productId, updateProductModel);

            // update product core to sql server
            ImageStoreModel? coverImageModel = imgDict.ContainsKey("coverImage") ? imgDict["coverImage"] : null;
            await _productRepo.UpdateProduct(productId, updateProductModel, coverImageModel);
            await _unitOfWork.SaveAsync();

            // update product info spec to mongodb
            var productInfo = updateProductModel.ToProductSpecInfoUpdate(productId, imgDict);
            // Console.WriteLine("ProductInfo: " + productInfo.ToJson());
            await _productInfoMongoRepo.UpdateProductInfo(productId, productInfo);

            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Update product success",
                Data = productInfo
            });
        }

        [HttpDelete("/api/v1/products/{productId}/delete")]
        public async Task<IActionResult> DeleteProductById([FromRoute] int productId)
        {
            try {
                _productRepo.DeleteProductByIdAsync(productId);
            } catch (Exception ex){
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 404,
                    Message = ex.Message
                });
            }
            await _unitOfWork.SaveAsync();
            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Delete product success"
            });
        }

        private async Task<Dictionary<string, ImageStoreModel>> UploadProductImageAsync(AddProductRequest addProductRequest)
        {
            var lengthOfSpecItems = addProductRequest.SpecAttributes.FirstOrDefault(s => s.IsPrimary).SpecItems.Count;
            Dictionary<string, ImageStoreModel> imagesNameDict = new Dictionary<string, ImageStoreModel>(); // first index is cover image

            // upload cover image
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var coverImageName = UtilityServices.GenerateRandomString(6) + timeStamp;
            var coverImageUrl = await _imagesBlobServices.UploadImageAsync(
                _imagesBlobServices.ConvertToFileStreamModel(coverImageName, addProductRequest.CoverImage)
            );
            imagesNameDict.Add("coverImage", new ImageStoreModel
            {
                Name = coverImageName,
                Url = coverImageUrl
            });

            // upload special info images
            if (addProductRequest.SpecAttributes.Count > 0)
            {
                for (int i = 0; i < lengthOfSpecItems; i++)
                {
                    var specialImageName = UtilityServices.GenerateRandomString(6) + timeStamp;
                    var specItem = addProductRequest.SpecAttributes.FirstOrDefault(s => s.IsPrimary).SpecItems[i];
                    if (specItem == null)
                    {
                        continue;
                    }
                    var url = await _imagesBlobServices.UploadImageAsync(
                        _imagesBlobServices.ConvertToFileStreamModel(specialImageName, specItem.SpecImage)
                    );
                    imagesNameDict.Add(specItem.SpecValue, new ImageStoreModel
                    {
                        Name = specialImageName,
                        Url = url
                    });
                }
            }

            return imagesNameDict;
        }
        private async Task<Dictionary<string, ImageStoreModel>> UpdateProductImageAsync(int productId, AddProductRequest updateProductModel)
        {

            Dictionary<string, ImageStoreModel> imagesNameDict = new Dictionary<string, ImageStoreModel>(); // first index is cover image
            var productSpec = await _productInfoMongoRepo.GetProductSpecInfoByProductIdAsync(productId);
            var product = await _productRepo.GetProductByIdAsync(productId);

            // upload cover image
            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (updateProductModel.CoverImage != null)
            {
                var coverImageName = UtilityServices.GenerateRandomString(6) + timeStamp;
                var coverImageUrl = await _imagesBlobServices.UpdateImageAsync(
                    // If Upload but not update productImages of database, the data will be old
                    product?.AvtImgName == null ? "" : product.AvtImgName,
                    _imagesBlobServices.ConvertToFileStreamModel(coverImageName, updateProductModel.CoverImage)
                );
                // Can optimize
                imagesNameDict.Add("coverImage", new ImageStoreModel
                {
                    Name = coverImageName,
                    Url = coverImageUrl,
                    isUpdating = true
                });
            }

            // upload special info images
            if (updateProductModel.SpecAttributes == null)
            {
                return imagesNameDict;
            }

            var lengthOfSpecItems = updateProductModel.SpecAttributes.FirstOrDefault(s => s.IsPrimary)?.SpecItems?.Count;
            if (lengthOfSpecItems == null)
            {
                return imagesNameDict;
            }

            var updateSpecItem = new List<EPlatform_API.DTOs.ProductDTOs.SpecItem>();
            for (int i = 0; i < lengthOfSpecItems; i++)
            {
                // item DTO
                var specItem = updateProductModel.SpecAttributes[0].SpecItems?[i];
                var specialImageName = UtilityServices.GenerateRandomString(6) + timeStamp;
                if (specItem == null)
                {
                    continue;
                }

                if (productSpec.SpecInfos[0]?.SpecItems?.Count < lengthOfSpecItems)
                {
                    bool isStore = false;
                    foreach (var oldItem in productSpec.SpecInfos[0]?.SpecItems)
                    {
                        if (oldItem.SpecValue == specItem.SpecValue)
                        {
                            isStore = true;
                            break;
                        }
                    }
                    if (isStore == false && specItem.SpecImage != null)
                    {
                        var newUrl = await _imagesBlobServices.UploadImageAsync(
                            _imagesBlobServices.ConvertToFileStreamModel(specialImageName, specItem.SpecImage)
                        );

                        imagesNameDict.Add(specItem.SpecValue, new ImageStoreModel
                        {
                            Name = specialImageName,
                            Url = newUrl
                        });
                        continue;
                    }
                }

                // item in db
                var specItemInDb = productSpec.SpecInfos[0]?.SpecItems?.FirstOrDefault(si => si.SpecValue == specItem.SpecValue);
                var oldName = specItemInDb?.SpecImageName;
                var oldUrl = specItemInDb?.SpecImageUrl;

                if (specItem.SpecValue == null)
                {
                    continue;
                }

                // if client not send image update
                if (updateProductModel.SpecAttributes[0].SpecItems?[i].SpecImage == null)
                {
                    imagesNameDict.Add(specItem.SpecValue, new ImageStoreModel
                    {
                        Name = oldName,
                        Url = oldUrl
                    });
                    continue;
                }

                if (specItem.SpecImage == null)
                {
                    continue;
                }

                var url = await _imagesBlobServices.UpdateImageAsync(
                    oldName,
                    _imagesBlobServices.ConvertToFileStreamModel(specialImageName, specItem.SpecImage)
                );

                if (url == null)
                {
                    continue;
                }

                imagesNameDict.Add(specItem.SpecValue, new ImageStoreModel
                {
                    Name = specialImageName,
                    Url = url,
                    isUpdating = true
                });
            }

            return imagesNameDict;
        }

    }
}
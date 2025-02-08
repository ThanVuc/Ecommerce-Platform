using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.AdminDTOs;
using EPlatform_API.DTOs.AdminDTOs.Users;
using EPlatform_API.DTOs.AuthDTOs;
using EPlatform_API.DTOs.ProductDTOs;
using EPlatform_API.DTOs.ShopDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.Models;
using EPlatform_API.Models.ShopOwners;
using Microsoft.AspNetCore.Identity;

namespace EPlatform_API.Mappers
{
    // public string? Username {get; set;}
    // public string? PhoneNumber {get; set;}
    // public string PasswordHash {get; set;} = string.Empty;
    // public string Address {get; set;} = string.Empty;
    // public string First {get; set;} = string.Empty;
    // public string Last {get; set;} = string.Empty;
    public static class DTOMappers
    {
        public static AppUser ToUser(this RegisterRequestModel registerModel)
        {
            return new AppUser()
            {
                UserName = registerModel.Username,
                Email = registerModel.Username,
                PhoneNumber = registerModel.PhoneNumber,
                PasswordHash = registerModel.Password,
                HomeAddress = registerModel.Address,
                First = registerModel.First,
                Last = registerModel.Last,
                Create = DateTime.Now
            };
        }

        public static AppUser ToUser(this LoginRequestModel loginModel)
        {
            return new AppUser()
            {
                UserName = loginModel.Username,
                Email = loginModel.Username,
                PasswordHash = loginModel.Password,
            };
        }

        public static AppUser ToUser(this CreateUserRequestModel createUserModel)
        {
            return new AppUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Username,
                PhoneNumber = createUserModel.PhoneNumber,
                PasswordHash = createUserModel.Password,
                HomeAddress = createUserModel.Address,
                First = createUserModel.First,
                Last = createUserModel.Last,
                Create = DateTime.Now
            };
        }

        public static UserDetailResponseModel ToUserDetailResponse(this AppUser user)
        {
            return new UserDetailResponseModel
            {
                Address = user.HomeAddress,
                Age = user.Age,
                AvatarImageUrl = user.AvatarImageUrl,
                Created = user.Create,
                First = user.First,
                Last = user.Last,
                National = user.National,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                Gender = user.Gender
            };
        }

        public static Shop ToShop(this CreateShopRequest createShopRequest)
        {
            return new Shop()
            {
                ShopId = createShopRequest.ShopId,
                Name = createShopRequest.Name,
                CreatedAt = createShopRequest.CreatedAt,
                UpdatedAt = createShopRequest.UpdatedAt,
                Slug = UtilityServices.GenerateSlug(createShopRequest.Name),
                PickUpAddress = createShopRequest.PickUpAddress,
                ShopAddress = createShopRequest.ShopAddress,
                Phone = createShopRequest.Phone,
                Email = createShopRequest.Email,
                InvoiceEmail = createShopRequest.InvoiceEmail,
                TaxesCode = createShopRequest.TaxesCode,
                IdentificationNumber = createShopRequest.IdentificationNumber
            };
        }

        public static Product ToProduct(this AddProductRequest addProductRequest)
        {
            return new Product()
            {
                Name = addProductRequest.Name,
                Description = addProductRequest.Description,
                Price = addProductRequest.Price,
                IsPublic = addProductRequest.IsPublic,
                CategoryId = addProductRequest.CategoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Slug = UtilityServices.GenerateSlug(addProductRequest.Name),
                AvtImgUrl = @"https://sinhnguyen417.blob.core.windows.net/public-images/600x400.png"
            };
        }

        public static GetCategoriesResponse ToCategoriesResponse(this Category category)
        {
            return new GetCategoriesResponse()
            {
                CategoryId = category.CategoryId,
                Name = category.Name == null ? "empty" : category.Name,
                isNext = category.SubCategories == null ? false : category.SubCategories.Count > 0 ? true : false
            };
        }

        public static ProductSpecInfo ToProductSpecInfo(this AddProductRequest addProductRequest, int productId, Dictionary<string, string> urlDict)
        {
            var specInfo = new List<Spec>();
            if (addProductRequest.SpecAttributes == null)
            {
                return new ProductSpecInfo()
                {
                    ProductId = productId,
                    SpecInfos = specInfo
                };
            }

            try
            {
                for (int i = 0; i < addProductRequest.SpecAttributes.Count; i++)
                {
                    if (addProductRequest.SpecAttributes[i].SpecItems == null)
                    {
                        specInfo.Add(new Spec()
                        {
                            SpecName = addProductRequest.SpecAttributes[i].SpecName,
                            IsPrimary = addProductRequest.SpecAttributes[i].IsPrimary,
                            SpecItems = null
                        });
                        continue;
                    }

                    var specItems = new List<SpecItem>();
                    for (int j = 0; j < addProductRequest.SpecAttributes[i].SpecItems.Count; j++)
                    {
                        var specValue = addProductRequest.SpecAttributes[i].SpecItems[j].SpecValue;
                        specItems.Add(new SpecItem()
                        {
                            SpecValue = specValue,
                            SpecImageUrl = urlDict[specValue]
                        });
                    }
                    specInfo.Add(new Spec()
                    {
                        SpecName = addProductRequest.SpecAttributes[i].SpecName,
                        IsPrimary = addProductRequest.SpecAttributes[i].IsPrimary,
                        SpecItems = specItems
                    });

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Line approximately 174 - DTOMapper -" + e.Message);
            }

            var specInfoInventories = new List<SpecInventory>();

            try{
                for (int i = 0; i < addProductRequest.SpecInventories.Count; i++)
                {
                    specInfoInventories.Add(new SpecInventory()
                    {
                        PrimarySpecValueName = addProductRequest.SpecInventories[i].PrimarySpecValueName,
                        SubSpecValueName = addProductRequest.SpecInventories[i].SubSpecValueName,
                        Inventory = addProductRequest.SpecInventories[i].Inventory
                    });
                }
            } catch (Exception e)
            {
                Console.WriteLine("Line approximately 183 - DTOMapper -" + e.Message);
            }


            return new ProductSpecInfo()
            {
                ProductId = productId,
                SpecInfos = specInfo,
                SpecInfoInventories = specInfoInventories
            };
        }
    }
}
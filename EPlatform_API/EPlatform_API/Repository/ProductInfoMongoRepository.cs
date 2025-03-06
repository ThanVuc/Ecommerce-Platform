using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using EPlatform_API.Models.ShopOwners;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MongoDB.Driver;

namespace EPlatform_API.Repository
{
    public class ProductInfoMongoRepository : MongoRepository<ProductSpecInfo>
    {
        private readonly IMongoCollection<ProductSpecInfo> _productSpecInfoCollection;
        public ProductInfoMongoRepository(IMongoDatabase database, ILoggingService loggingService) : base(database, loggingService)
        {
            _productSpecInfoCollection = database.GetCollection<ProductSpecInfo>(MongoDbCollections.ProductSpecInfo);
        }

        public async Task<ProductSpecInfo> GetProductSpecInfoByProductIdAsync(int productId)
        {
            try
            {
                var productSpecInfo = await _productSpecInfoCollection.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
                return productSpecInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //update
        public async Task<bool> UpdateProductInfo(
            int productId,
            ProductSpecInfo productInfo
        )
        {
            try
            {
                var filter = Builders<ProductSpecInfo>.Filter.Eq(p => p.ProductId, productId);
                var update = Builders<ProductSpecInfo>.Update
                    .Set(p => p.SpecInfos, productInfo.SpecInfos)
                    .Set(p => p.SpecInfoInventories, productInfo.SpecInfoInventories);

                await _productSpecInfoCollection.UpdateOneAsync(filter, update);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetAvailableInventory(int productId ,string primary, string? sub = null)
        {
            try
            {
                var productSpecInfo = await _productSpecInfoCollection.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
                SpecInventory? inventory;
                if (sub == null){
                    inventory = productSpecInfo.SpecInfoInventories.FirstOrDefault(p => p.PrimarySpecValueName == primary && p.SubSpecValueName == null);
                    if (inventory != null)
                    {
                        return inventory.Inventory;
                    }
                    else
                    {
                        return 0;
                    }
                } else {
                    inventory = productSpecInfo.SpecInfoInventories.FirstOrDefault(p => p.PrimarySpecValueName == primary && p.SubSpecValueName == sub);
                    if (inventory != null)
                    {
                        return inventory.Inventory;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
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


    }
}
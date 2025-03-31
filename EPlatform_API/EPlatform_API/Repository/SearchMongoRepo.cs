using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.IServices;
using EPlatform_API.Models.ShopOwners;
using MongoDB.Driver;

namespace EPlatform_API.Repository
{
    public class SearchMongoRepo : MongoRepository<AutocompleteProduct>
    {
        private readonly IMongoCollection<AutocompleteProduct> _searchProductAnalysicCollection;
        private readonly IMongoCollection<AutocompleteProduct> _searchAutocompleteCollection;
        public SearchMongoRepo(IMongoDatabase database, ILoggingService loggingService) : base(database, loggingService)
        {
            _searchProductAnalysicCollection = database.GetCollection<AutocompleteProduct>(ExtensionMethods.MongoDbCollections.SearchProductAnalysic);
            _searchAutocompleteCollection = database.GetCollection<AutocompleteProduct>(ExtensionMethods.MongoDbCollections.AutoComplete);
            CreateTTLIndex().Wait(); // Ensure the TTL index is created on initialization
        }


        // insert or update frequences
        public async Task<bool> InsertOrUpdateSearchProductAnalysic(string productName)
        {
            try
            {
                var filter = Builders<AutocompleteProduct>.Filter.Eq(p => p.Name, productName);
                var existingProduct = await _searchProductAnalysicCollection.Find(filter).FirstOrDefaultAsync();

                if (existingProduct == null)
                {
                    var newProduct = new AutocompleteProduct
                    {
                        Name = productName,
                        Frequences = 1,
                        NearestAccess = DateTime.UtcNow
                    };
                    await _searchProductAnalysicCollection.InsertOneAsync(newProduct);
                }
                else
                {
                    var update = Builders<AutocompleteProduct>.Update
                        .Set(p => p.NearestAccess, DateTime.UtcNow)
                        .Inc(p => p.Frequences, 1);

                    await _searchProductAnalysicCollection.UpdateOneAsync(filter, update);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // get all products for autocomplete
        public async Task<List<AutocompleteProduct>> GetAllProductsForAutocomplete(string prefix)
        {
            var filter = Builders<AutocompleteProduct>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression($"^{prefix}", "i"));
            var sort = Builders<AutocompleteProduct>.Sort.Descending(p => p.Frequences);
            var products = await _searchAutocompleteCollection
            .Find(filter)
            .Sort(sort)
            .Limit(10)
            .ToListAsync();
            return products;
        }

        // create TTL index for nearest-access field
        private async Task CreateTTLIndex()
        {
            var indexKeys = Builders<AutocompleteProduct>.IndexKeys.Ascending(w => w.NearestAccess);
            var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(7) };
            var indexModel = new CreateIndexModel<AutocompleteProduct>(indexKeys, indexOptions);
            
            await _searchProductAnalysicCollection.Indexes.CreateOneAsync(indexModel);
        }

    }
}
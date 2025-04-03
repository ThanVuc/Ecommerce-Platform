using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.IServices;
using EPlatform_API.Models.ShopOwners;
using MongoDB.Driver;

namespace EPlatform_API.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IMongoCollection<AutocompleteProduct> _searchProductAnalysicCollection;
        private readonly IMongoCollection<AutocompleteProduct> _autocompleteProductCollection;
        private readonly IConfiguration _configuration;

        public SynchronizationService(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = configuration.GetConnectionString("Cloud_MongoDB");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(configuration["MongoDB:Database"]);

            _searchProductAnalysicCollection = database.GetCollection<AutocompleteProduct>(ExtensionMethods.MongoDbCollections.SearchProductAnalysic);
            _autocompleteProductCollection = database.GetCollection<AutocompleteProduct>(ExtensionMethods.MongoDbCollections.AutoComplete);
            CreateTTLIndex().Wait(); // Ensure the TTL index is created on initialization
        }

        // insert or update frequences
        public async Task<bool> InsertOrUpdateSearchProductAnalysic(AutocompleteProduct autocomplete)
        {
            try
            {
                var filter = Builders<AutocompleteProduct>.Filter.Eq(p => p.Name, autocomplete.Name);
                var existingProduct = await _autocompleteProductCollection.Find(filter).FirstOrDefaultAsync();

                if (existingProduct == null)
                {
                    var newProduct = new AutocompleteProduct
                    {
                        Name = autocomplete.Name,
                        Frequences = 1,
                        NearestAccess = DateTime.UtcNow
                    };
                    await _autocompleteProductCollection.InsertOneAsync(newProduct);
                }
                else
                {
                    var update = Builders<AutocompleteProduct>.Update
                        .Set(p => p.NearestAccess, DateTime.UtcNow)
                        .Set(p => p.Frequences, autocomplete.Frequences);

                    await _autocompleteProductCollection.UpdateOneAsync(filter, update);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    
        public async Task UpdateAutocompleteData(){
            try
            {
                // get analytic data from searchProductAnalysic collection
                var filter = Builders<AutocompleteProduct>.Filter.Empty;
                var searchProductAnalysic = await _searchProductAnalysicCollection
                .Find(filter)
                .Sort(Builders<AutocompleteProduct>.Sort.Descending(p => p.Frequences))
                .Limit(1000) // Limit to top 1000 products based on frequency
                .ToListAsync();

                // update
                foreach (var productItem in searchProductAnalysic)
                {
                    await InsertOrUpdateSearchProductAnalysic(productItem);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
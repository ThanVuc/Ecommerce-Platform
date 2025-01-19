using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.IRepository;
using MongoDB.Driver;

namespace EPlatform_API.Repository
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<T>(typeof(T).Name);
        }

        public virtual async Task CreateAsync(T document)
        {
            await _collection.InsertOneAsync(document);
        }

        public virtual async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id",id);
            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0){
                throw new Exception("Document not found");
            }
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(string id){
            var filter = Builders<T>.Filter.Eq("_id",id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public virtual async Task Update(string id, T document)
        {
            var filter = Builders<T>.Filter.Eq("_id",id);
            var result = await _collection.ReplaceOneAsync(filter,document);
            if (result.MatchedCount == 0){
                throw new Exception("Line 34 - MongoDB, Updated Fail");
            }
        }
    }
}
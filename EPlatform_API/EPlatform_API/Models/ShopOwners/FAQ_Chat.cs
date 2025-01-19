using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EPlatform_API.Models.ShopOwners
{
    public class FAQ_Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonRequired]
        public int ShopId { get; set; }

        public string? Question { get; set; }
        public string? Answer { get; set; }

        [BsonRequired]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int ViewCount { get; set; }

        public ICollection<string>? Tags { get; set; }
    }
}
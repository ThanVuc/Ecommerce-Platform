using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EPlatform_API.Models.ShopOwners
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonRequired]
        public string? CustomerId { get; set; }

        [BsonRequired]
        public string? ShopId { get; set; }

        public string? Message { get; set; }

        [BsonRequired]
        public DateTime Time { get; set; }
    }
}
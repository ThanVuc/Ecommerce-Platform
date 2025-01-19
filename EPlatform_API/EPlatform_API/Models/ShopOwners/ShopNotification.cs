using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EPlatform_API.Models.ShopOwners
{
    public class ShopNotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonRequired]
        public int ShopId { get; set; }

        [BsonRequired]
        public string? ActorId { get; set; }

        [BsonRequired]
        public string? Title { get; set; }
        [BsonRequired]
        public string? Message { get; set; }

        public bool IsRead { get; set; }

        [BsonRequired]
        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }
        public string? ActionLink { get; set; }
        public bool IsDeleted { get; set; }
    }
}
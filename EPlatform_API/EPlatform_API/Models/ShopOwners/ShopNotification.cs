using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EPlatform_API.Models.ShopOwners
{
    [Authorize]
    public class ShopNotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [BsonRequired]
        public string? ShopId { get; set; }

        [BsonRequired]
        public string? ActorId { get; set; }

        [BsonRequired]
        public string? Message { get; set; }

        [BsonRequired]
        public DateTime CreatedAt { get; set; }

        public string? ActionLink { get; set; }
    }
}
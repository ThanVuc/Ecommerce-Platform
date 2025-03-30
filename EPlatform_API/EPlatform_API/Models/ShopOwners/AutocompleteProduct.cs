using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace EPlatform_API.Models.ShopOwners
{
    [BsonIgnoreExtraElements]
    public class AutocompleteProduct
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)] 
        public string? _id { get; set; }

        [BsonRequired]
        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonRequired]
        [BsonElement("frequences")]
        public int Frequences { get; set; } = 0;
    }
}
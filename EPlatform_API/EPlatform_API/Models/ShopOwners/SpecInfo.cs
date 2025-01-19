using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace EPlatform_API.Models.ShopOwners
{
    public class SpecInfo
    {
        [BsonRequired]
        public string? Key { get; set; }
        [BsonRequired]
        public object? Value { get; set; }
    }
}
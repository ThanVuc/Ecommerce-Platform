using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace EPlatform_API.Models.ShopOwners
{
    public class ProductSpecInfo
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        public int ProductId { get; set; }
        
        [BsonRequired]
        public List<Spec> SpecInfos { get; set; } = new List<Spec>();
        
        [BsonRequired]
        public List<SpecInventory> SpecInfoInventories { get; set; } = new List<SpecInventory>();
    }
}

public class Spec{
    public string? SpecName { get; set; }
    public bool IsPrimary { get; set; } = false;
    public List<SpecItem>? SpecItems { get; set; }
}

public class SpecItem
{
    public string? SpecValue { get; set; }
    public string? SpecImageUrl { get; set; }
    public string? SpecImageName { get; set; }
}

// Store the inventory of each spec
public class SpecInventory{
    public string? PrimarySpecValueName { get; set; }
    public string? SubSpecValueName { get; set; }
    public int Inventory { get; set; }
}
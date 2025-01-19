using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ProductDTOs
{
    public class AddProductRequest
    {
        public string? ShopId { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool IsPublic { get; set; }
        public int? Quantity { get; set; }
        public int WarehouseId { get; set; }
        public int CategoryId {get; set;}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ProductDTOs
{
    public class ProductDetailResponse
    {
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsPublic { get; set; }
        public List<SpecAttributeUpdate>? SpecAttributes { get; set; }
        public List<SpecInventoryUpdate>? SpecInventories { get; set; }
        public int WarehouseId { get; set; }
        public int TotalInventory { get; set; }
        public string? Slug { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
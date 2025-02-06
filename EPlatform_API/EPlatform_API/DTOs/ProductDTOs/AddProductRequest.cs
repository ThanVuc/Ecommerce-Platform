using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ProductDTOs
{
    public class AddProductRequest
    {
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsPublic { get; set; }
        public List<SpecAttribute>? SpecAttributes { get; set; }
        public List<SpecInventory>? SpecInventories { get; set; }
        public int WarehouseId { get; set; }
        public int TotalInventory { get; set; }
        public IFormFile? CoverImage { get; set; }
    }

    public class SpecAttribute
    {
        public string? SpecName { get; set; }
        public bool IsPrimary { get; set; }
        public List<SpecItem>? SpecItems { get; set; }
    }

    public class SpecItem
    {
        public string? SpecValue { get; set; }
        public IFormFile? SpecImage { get; set; }
    }

    public class SpecInventory
    {
        public string? PrimarySpecValueName { get; set; }
        public string? SubSpecValueName { get; set; }
        public int Inventory { get; set; }
    }
}
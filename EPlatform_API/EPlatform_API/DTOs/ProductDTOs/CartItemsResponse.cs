using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ProductDTOs
{
    public class CartItemsResponse
    {
        public int? CartItemId { get; set; }
        public string? ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public int AvailableQuantity { get; set; }
        public int Quantity { get; set; }
        public string? ProductAvtImg { get; set; }
        public int ProductId { get; set; }
        public string? SpecInfo { get; set; }
        public string? ShopId { get; set; }
        public string? ShopName { get; set; }
        public string? ShopLogoUrl { get; set; }
    }
}
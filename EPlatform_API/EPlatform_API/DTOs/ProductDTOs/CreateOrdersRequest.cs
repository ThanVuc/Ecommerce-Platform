using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Mozilla;

namespace EPlatform_API.DTOs.ProductDTOs
{
    public class CreateOrdersRequest
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ShippingAddress { get; set; }
        public List<CartItemOfOrder>? CartItems { get; set; }
    }

    public class CartItemOfOrder
    {
        public string? ShopId { get; set; }
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? SpecInfo { get; set; }
        
    }
}
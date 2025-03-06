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
        public string? Address { get; set; }
        public List<CartItemOfOrder>? CartItems { get; set; }
    }

    public class CartItemOfOrder
    {
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public string? SpecInfo { get; set; }
    }
}
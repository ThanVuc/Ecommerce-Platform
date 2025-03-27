using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.OrderDTOs
{
    public class PurchaseOrdersResponse
    {
        public int OrderId { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime CreateAt { get; set; }
        public string? ShopAvt { get; set; }
        public string? ShopName { get; set; }
        public string? PaymentName { get; set; }
        public List<ProductModel>? Products { get; set; } // Updated to include a list of products
    }

    public class ProductModel // Define the product model
    {
        public string? ProductId { get; set; }
        public string? ProductAvtImg { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
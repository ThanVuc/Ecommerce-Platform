using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.OrderDTOs
{
    public class OrderDetailResponse
    {
        public int OrderId { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime CreateAt { get; set; }
        public string? AccountName { get; set; }
        public int OrderNums { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CustomerName { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ShippingPhone { get; set; }
        public List<ProductDetail>? Products { get; set; }
    }

    public class ProductDetail
    {
        public string? AvtImg { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
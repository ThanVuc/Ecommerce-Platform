using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.OrderDTOs
{
    public class NotificationOrderRequest
    {
        public string? ShopId { get; set; }
        public int OrderId { get; set; }
    }
}
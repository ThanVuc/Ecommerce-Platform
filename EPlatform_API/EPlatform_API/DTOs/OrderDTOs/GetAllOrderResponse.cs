using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.OrderDTOs
{
    public class GetAllOrderResponse
    {
        public int OrderId { get; set; }
        public string? CustomerEmail { get; set; }
        public string? Payment { get; set; }
        public string? ProductNames { get; set; }
        public DateTime CreateAt { get; set; }
        public int OrderStatusId { get; set; }
        public string? OrderStatusName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.OrderDTOs
{
    public class UpdateOrderStatusRequest
    {
        public int OrderId { get; set; }
        public string? Status { get; set; }
        public int StatusId { get; set; }
    }
}
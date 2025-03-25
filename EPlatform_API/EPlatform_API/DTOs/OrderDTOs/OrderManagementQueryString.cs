using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models;

namespace EPlatform_API.DTOs.OrderDTOs
{
    public class OrderManagementQueryString : QueryStringParameters
    {
        public int? OrderStatusId { get; set; }
    }
}
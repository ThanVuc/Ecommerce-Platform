using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models;

namespace EPlatform_API.DTOs.ProductDTOs
{
    public class SearchProductQueryDTO : QueryStringParameters
    {
        public int CategoryId { get; set; }
    }
}
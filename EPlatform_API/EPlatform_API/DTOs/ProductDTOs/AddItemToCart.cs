using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ProductDTOs
{
    public class AddItemToCart
    {
        public int Quantity { get; set; }
        public string? SpecInfo { get; set; }
        public int ProductId { get; set; }
    }
}
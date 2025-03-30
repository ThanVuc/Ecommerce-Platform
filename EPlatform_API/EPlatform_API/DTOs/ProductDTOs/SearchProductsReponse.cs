using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ProductDTOs
{
    public class SearchProductsReponse
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? AvtImgUrl { get; set; }
        public int SoldQuantity { get; set; }
        public string? Slug { get; set; }
    }
}
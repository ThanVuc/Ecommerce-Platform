using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ShopDTOs
{
    public class GetCategoriesResponse
    {
        public string Name {get; set;}
        public int CategoryId {get; set;}
        public bool isNext {get; set;}
    }
}
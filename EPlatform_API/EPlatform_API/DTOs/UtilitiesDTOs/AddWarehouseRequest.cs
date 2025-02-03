using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.UtilitiesDTOs
{
    public class AddWarehouseRequest
    {
        public int WarehouseId { get; set; }
        public string? Name { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Location { get; set; }
        public int? Capability { get; set; }
    }
}
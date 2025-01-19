using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.ShopDTOs
{
    public class CreateShopRequest
    {
        public string? ShopId { get; set; }

        public string? Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? Slug { get; set; }

        public string? PickUpAddress { get; set; }

        public string? ShopAddress { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }
        public string? InvoiceEmail { get; set; }

        public string? TaxesCode { get; set; }
        public string? IdentificationNumber { get; set; }
    }
}
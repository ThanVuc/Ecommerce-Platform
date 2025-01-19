using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class ShipmentCarrier
    {
        [Key]
        public int CarrierId {get; set;}

        [Required]
        [StringLength(64)]
        public string? Name {get; set;}

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public Decimal ShippingCost {get; set;}

        public ICollection<Shipment>? Shipments {get; set;}

    }
}
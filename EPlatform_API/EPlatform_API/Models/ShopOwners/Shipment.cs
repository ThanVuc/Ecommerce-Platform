using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class Shipment
    {
        [Key]
        [StringLength(10)]
        public int ShipmentId { get; set; }

        [ForeignKey("Order")]
        public int OrderId {get; set;}

        [ForeignKey("Carrier")]
        public int CarrierId {get; set;}

        [Required]
        public DateTime ShipmentDate { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [StringLength(500)]
        public string? ShippingAddress { get; set; }

        [Required]
        [StringLength(64)]
        public string? ShippingMethod { get; set; }

        public string? TrackingNumber { get; set; }

        [Required]
        [StringLength(64)]
        public string? ShipStatus { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ShipmentCarrier? Carrier {get; set;}
        public Order? Order {get; set;}
    }
}
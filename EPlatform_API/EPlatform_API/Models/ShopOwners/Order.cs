using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class Order
    {
        [Key]
        [StringLength(10)]
        public int OrderId { get; set; }

        [Required]
        // [ForeignKey("Shop")]
        // [StringLength(10)]
        public string? ShopId { get; set; }

        [Required]
        [ForeignKey("Customer")]
        // [StringLength(10)]
        public string? CustomerId { get; set; }

        [Required]
        [ForeignKey("Shipment")]
        public string? ShipmentId {get; set;}

        [Required]
        [ForeignKey("OrderStatus")]
        public int OrderStatusId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAmount { get; set; }

        [Required]
        public string? ShippingAddress { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? CompleteOrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ShipmentCost { get; set; }

        [StringLength(100)]
        public string? PaymentMethod { get; set; }

        public OrderStatus? OrderStatus { get; set; }
        public ICollection<OrderProduct>? OrderProducts { get; set; }
        public AppUser? Customer {get; set;}
        public Shipment? Shipment {get; set;}
    }
}
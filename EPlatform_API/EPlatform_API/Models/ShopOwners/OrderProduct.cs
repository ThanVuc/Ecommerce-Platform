using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class OrderProduct
    {
        [Key,Column(Order = 0)]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Key,Column(Order = 1)]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public Decimal ProductsPrice { get; set; }
        public string? SpecInfo { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
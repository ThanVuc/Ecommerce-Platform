using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class ProductDiscount
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Key, Column("DiscountId")]
        [ForeignKey("Discount")]
        public int DiscountId { get; set; }

        public Product? Product { get; set; }
        public Discount? Discount { get; set; }
    }
}
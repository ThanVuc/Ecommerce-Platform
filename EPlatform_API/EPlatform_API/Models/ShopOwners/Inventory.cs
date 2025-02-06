using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class Inventory
    {
        [Key]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ForeignKey("WareHouse")]
        public int WareHouseId {get; set;}
        public int? Quantity { get; set; }
        public int? SoldQuantity { get; set; }
        public int? AvailableQuantity { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsAvailable { get; set; }
        public Product? Product { get; set; }
        public WareHouse? WareHouse {get; set;}
    }
}
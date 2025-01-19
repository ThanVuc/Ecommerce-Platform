using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class WareHouse
    {
        [Key]
        [StringLength(10)]
        public int WarehouseId { get; set; }

        [Required]
        [StringLength(256)]
        public string? Name { get; set; }

        [Required]
        [StringLength(12)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(256)]
        public string? Email { get; set; }

        [Required]
        [StringLength(500)]
        public string? Location { get; set; }

        public int? Capability { get; set; }

        public ICollection<Inventory>? Inventory {get; set;}
    }
}
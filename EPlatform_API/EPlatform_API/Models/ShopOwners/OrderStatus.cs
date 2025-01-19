using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class OrderStatus
    {
        [Key]
        public int OrderStatusId { get; set; }

        [Required]
        [StringLength(64)]
        public string? StatusName { get; set; }

        public string? Description { get; set; }
        public bool IsFinal { get; set; }
    }
}
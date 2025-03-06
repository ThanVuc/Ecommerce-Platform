using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class Cart
    {
        public int CartId { get; set; }
        public AppUser? Customer { get; set; }
        [ForeignKey("Customer")]
        public string? CustomerId { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }
}
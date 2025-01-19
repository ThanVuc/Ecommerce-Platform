using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models.ShopOwners;
using Microsoft.AspNetCore.Identity;

namespace EPlatform_API.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "Home Address is Required")]
        public string? HomeAddress {get; set;}
        public string? First {get; set;}
        public string? Last {get; set;}
        public string AvatarImageUrl {get; set;} = "";
        public DateTime Create {get; set;} = DateTime.Now;
        public string? National {get; set;}
        [Range(minimum:0, maximum: 150)]
        public int Age {get; set;}
        public bool Gender {get; set;}
        public Shop? Shop {get; set;}
        public ICollection<Order>? Orders {get; set;}
    }
}
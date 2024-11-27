using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AdminDTOs.Users
{
    public class UserDetailResponseModel
    {
        public List<string>? Roles {get; set;}
        public string? Username {get; set;}
        public string? Address {get; set;}
        public string? First {get; set;}
        public string? Last {get; set;}
        public string? AvatarImageUrl {get; set;}
        public DateTime Create {get; set;} = DateTime.Now;
        public string? National {get; set;}
        public int Age {get; set;}
        public string? PhoneNumber {get; set;}
    }
}
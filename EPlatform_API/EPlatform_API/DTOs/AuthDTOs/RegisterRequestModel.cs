using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AuthDTOs
{
    public class RegisterRequestModel
    {
        [MaxLength(128)]
        [MinLength(3)]
        [Required]
        public string? Username {get; set;}
        [Phone]
        public string? PhoneNumber {get; set;}
        [Required]
        public string Password {get; set;} = string.Empty;
        public string ConfirmPassword {get; set;} = string.Empty;
        [Required]
        [MaxLength(512)]
        public string Address {get; set;} = string.Empty;
        [MaxLength(64)]
        public string First {get; set;} = string.Empty;
        [MaxLength(64)]
        public string Last {get; set;} = string.Empty;
    }
}
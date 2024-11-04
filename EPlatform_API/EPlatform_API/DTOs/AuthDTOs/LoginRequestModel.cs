using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AuthDTOs
{
    public class LoginRequestModel
    {
        [MaxLength(128)]
        [MinLength(3)]
        [Required]
        public string? Username {get; set;}
        [Required]
        public string Password {get; set;} = string.Empty;
    }
}
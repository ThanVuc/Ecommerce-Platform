using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AdminDTOs.Roles
{
    public class AddNewClaimRequestModel
    {
        [Required]
        public string ClaimType {get; set;} = string.Empty;
        [Required]
        public string ClaimValue {get; set;} = string.Empty;
    }
}
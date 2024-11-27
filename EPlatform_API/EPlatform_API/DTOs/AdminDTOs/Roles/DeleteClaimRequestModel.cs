using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AdminDTOs.Roles
{
    public class DeleteClaimRequestModel
    {
        [Required]
        public int Id {get; set;}
    }
}
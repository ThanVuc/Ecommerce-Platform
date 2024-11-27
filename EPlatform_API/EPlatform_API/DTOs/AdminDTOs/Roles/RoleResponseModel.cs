using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AdminDTOs.Roles
{
    public class RoleResponseModel
    {
        public string? RoleName {get; set;}
        public List<ClaimResponseModel>? Claims {get; set;}
    }
}
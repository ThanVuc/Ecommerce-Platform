using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AdminDTOs.Roles
{
    public class ClaimResponseModel
    {
        public int ClaimId {get; set;}
        public string ClaimType {get; set;} = string.Empty;
        public string ClaimValue {get; set;} = string.Empty;

    }
}
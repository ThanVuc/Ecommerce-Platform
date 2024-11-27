using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AdminDTOs.Users
{
    public class SettingRoleRequestModel
    {
        public string Id {get; set;} = string.Empty;
        public List<string> rolesId {get; set;} = new List<string>();
    }
}
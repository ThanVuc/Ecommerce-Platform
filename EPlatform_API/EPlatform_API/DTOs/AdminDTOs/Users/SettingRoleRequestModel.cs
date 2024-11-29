using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AdminDTOs.Users
{
    public class SettingRoleRequestModel
    {
        public List<string> rolesId {get; set;} = new List<string>();
    }
}
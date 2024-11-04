using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models
{
    public class Group
    {
        [Key]
        public int ID {get; set;}

        [MinLength(2)]
        [MaxLength(128)]
        [Required]
        public string GroupName {get; set;} = string.Empty;
        public List<GroupOfRole>? GroupOfRoles {get; set;}
    }
}
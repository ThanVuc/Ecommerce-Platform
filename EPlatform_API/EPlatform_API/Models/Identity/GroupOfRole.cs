using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models
{
    public class GroupOfRole
    {
        [ForeignKey("GroupID")]
        public Group? Group {get; set;}
        [ForeignKey("RoleID")]
        public Roles? Role {get; set;}
        [Required]
        public int GroupID {get; set;}
        [Required]
        public int RoleID {get; set;}
    }
}
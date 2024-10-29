using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models
{
    public class Users
    {
        [Key]
        public int ID {get; set;}
        [MaxLength(128)]
        [MinLength(3)]
        [Required]
        public string? Username {get; set;}

        [MaxLength(128)]
        [MinLength(3)]
        [Required]
        [EmailAddress]
        public string? Email {get; set;}
        [Phone]
        public string? PhoneNumber {get; set;}
        public bool ConfirmEmail {get; set;}
        public bool ConfirmPhone {get; set;}
        [Required]
        public string Password {get; set;} = string.Empty;
        [Required]
        [MaxLength(512)]
        public string Address {get; set;} = string.Empty;
        [MaxLength(64)]
        public string First {get; set;} = string.Empty;
        [MaxLength(64)]
        public string Last {get; set;} = string.Empty;
        public int? GroupID {get; set;}
        [ForeignKey("GroupID")]
        public Group? Group {get; set;}
        public DateTime Created {get; set;} = DateTime.Now;
    }
}
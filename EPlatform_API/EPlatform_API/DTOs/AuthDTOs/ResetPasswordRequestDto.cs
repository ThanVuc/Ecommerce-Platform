using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AuthDTOs
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [MinLength(3)]
        public string OldPassword {get; set;} = string.Empty;
        [Required]
        [MinLength(3)]
        public string NewPassword {get; set;} = string.Empty;
        [Required]
        [MinLength(3)]
        public string ConfirmNewPassword {get; set;} = string.Empty;
    }
}
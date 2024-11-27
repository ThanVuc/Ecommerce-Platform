using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.DTOs.AuthDTOs
{
    public class ForgotPasswordRequestModel
    {
        public string? VerifyCode {get; set;}
        [EmailAddress]
        [Required]
        public string? Email {get; set;}
    }
}
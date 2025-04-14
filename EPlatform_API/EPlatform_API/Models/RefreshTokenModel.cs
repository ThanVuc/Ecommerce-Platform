using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models
{
    public class RefreshTokenModel
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
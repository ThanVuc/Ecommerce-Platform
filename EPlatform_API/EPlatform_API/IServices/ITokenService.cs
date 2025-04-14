using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPlatform_API.Models;
using EPlatform_API.Services;
using EPlatform_API.Setting;
using Microsoft.AspNetCore.Identity;

namespace EPlatform_API.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(AppUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task WriteTokenToCookie(string refreshToken, string accessToken, HttpContext context, JwtSetting jwtSettings);
    
        Task<bool> IsTokenExpired(string token);

        Task RefreshExpiredToken(string token, HttpContext context, RedisServices redisServices, UserManager<AppUser> userManager);
    }
}
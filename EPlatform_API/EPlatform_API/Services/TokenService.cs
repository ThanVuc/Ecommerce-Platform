using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using EPlatform_API.Setting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EPlatform_API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _appDbContext;
        public TokenService(
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            AppDbContext appDbContext
        )
        {
            _configuration = configuration;
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        public async Task<string> GenerateAccessToken(AppUser user)
        {
            var claims = await GetListClaim(user);
            var jwtSettings = _configuration.GetSection("JWT").Get<JwtSetting>();
            if (jwtSettings == null)
            {
                throw new Exception("JWT settings not found in configuration.");
            }
            var secureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(
                secureKey,
                SecurityAlgorithms.HmacSha256
            );

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: signingCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            
            return tokenString;
        }

        public async Task<List<Claim>> GetListClaim(AppUser user){
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var rolesName = await _userManager.GetRolesAsync(user);
            foreach (var roleName in rolesName)
            {
                claims.Add(new Claim(ClaimTypes.Role,roleName));
                var role = await _appDbContext.Roles.Select(r => new {r.Id, r.Name}).FirstOrDefaultAsync(r => r.Name == roleName);
                var roleClaims = _appDbContext.RoleClaims.Where(rc => rc.RoleId == role.Id);
                foreach (var roleClaim in roleClaims){
                    claims.Add(new Claim(roleClaim.ClaimType,roleClaim.ClaimValue));
                }
            }

            claims.Add(new (ClaimTypes.NameIdentifier, user.Id.ToString()));
            
            return claims;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenSettings = _configuration.GetSection("JWT").Get<JwtSetting>();
            var TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(token, TokenValidationParameters, out securityToken);
            var JwtSecurityToken = securityToken as JwtSecurityToken;
            if (JwtSecurityToken == null || !JwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)){
                throw new SecurityTokenException("Invalid Token");
            }
            
            return principle;
        }


        // Ensure the security by using HttpOnly cookies - this will prevent JavaScript from accessing the cookies.
        public Task WriteTokenToCookie(string refreshToken, string accessToken, HttpContext context, JwtSetting jwtSettings)
        {
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings.RefreshTokenExpiryDays),
                Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.None,
                Path = "/"
            };

            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                // access token will be expired before cookies deleted, assign the same time with refresh token
                // to avoid the access token will be deleted when refresh token is still valid
                Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings.RefreshTokenExpiryDays),
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };
            context.Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
            context.Response.Cookies.Append("access_token", accessToken, accessCookieOptions);
            return Task.CompletedTask;
        }
    
        public Task<bool> IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return Task.FromResult(jwtToken.ValidTo < DateTime.UtcNow);
        }
    
        public async Task RefreshExpiredToken(string token, HttpContext context, RedisServices redisServices, UserManager<AppUser> userManager)
        {
            var accessToken = context.Request.Cookies["access_token"];
            var refreshToken = context.Request.Cookies["refresh_token"];
            if (accessToken == null){
                throw new UnauthorizedAccessException("Access Token doen not Exist");
            }

            var principle = GetPrincipalFromExpiredToken(accessToken);
            var userId = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var currentRefreshKey = _configuration["JWT:RefreshKey"] + userId;
            string currentRefreshToken = await redisServices.GetString(currentRefreshKey);
            // if the refresh token is expired, the redis will delete it, Needless to check
            if (string.IsNullOrEmpty(currentRefreshToken))
            {
                throw new UnauthorizedAccessException("Not Found Refresh Token");
            }

            if (user == null || currentRefreshToken != refreshToken)
            {
                throw new UnauthorizedAccessException("The Refresh Token is invalid");
            }

            var newAccessToken = await GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var jwtSetting = _configuration.GetSection("JWT").Get<JwtSetting>();

            await redisServices.SetString(
                currentRefreshKey,
                newRefreshToken,
                TimeSpan.FromDays(jwtSetting.RefreshTokenExpiryDays)
            );

            await WriteTokenToCookie(newRefreshToken, newAccessToken, context, jwtSetting);
        }
    }
}
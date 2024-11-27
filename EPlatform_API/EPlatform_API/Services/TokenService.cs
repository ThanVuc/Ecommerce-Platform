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
            var secureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
            var signingCredentials = new SigningCredentials(
                secureKey,
                SecurityAlgorithms.HmacSha256
            );
            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
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
            var TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"])),
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
    }
}
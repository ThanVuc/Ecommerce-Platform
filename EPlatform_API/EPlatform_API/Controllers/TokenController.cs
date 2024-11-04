using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.AuthDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace EPlatform_API.Controllers
{
    [Route("api/token")]
    public class TokenController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        private readonly DistributedCacheEntryOptions cacheOption;
        private readonly IConfiguration _configuration;


        public TokenController(
            IUnitOfWork unitOfWork,
            ILogger<AuthController> logger,
            IDistributedCache cache,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IConfiguration configuration
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
            _tokenService = tokenService;
            _configuration = configuration;
            cacheOption = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(7));
        }

        [HttpPost("refresh"), Authorize]
        public async Task<IActionResult> Refresh(JwtTokenRequestModel tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest(new ApiResponseStandard<JwtTokenRequestModel>
                {
                    Status = "Fail",
                    Message = "Bad Request",
                    Errors = new Dictionary<string, string>{
                        {"Token","Not Found Token Model"}
                    }
                });
            }
            var accessToken = tokenModel.AccessToken;
            var refreshToken = tokenModel.RefreshToken;
            var principle = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userName = principle.Identity.Name;

            var user = await _unitOfWork.UserRepo.FindAsync(u => u.Username == userName);
            var currentRefreshKey = _configuration["JWT:RefreshKey"] + user.ID;
            string currentRefreshToken;
            var rs = _cache.TryGetValue<string>(currentRefreshKey, out currentRefreshToken);
            // if the refresh token is expired, the redis will delete it, Needless to check
            if (!rs)
            {
                return StatusCode(404, new ApiResponseStandard<JwtTokenRequestModel>
                {
                    Status = "Fail",
                    Message = "Not Found",
                    Errors = new Dictionary<string, string>{
                        {"Token","Not Found Refresh Token"}
                    }
                });
            }

            _logger.LogError("Loggg:" + currentRefreshToken + " - " + refreshToken);

            if (user == null || currentRefreshToken != refreshToken)
            {
                return BadRequest(new ApiResponseStandard<JwtTokenRequestModel>
                {
                    Status = "Fail",
                    Message = "Bad Request",
                    Errors = new Dictionary<string, string>{
                        {"Token","The Refresh Token is invalid"}
                    }
                });
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principle.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _cache.SetAsync<string>(
                currentRefreshKey,
                newRefreshToken,
                cacheOption
            );

            return Ok(new ApiResponseStandard<JwtTokenReponseModel>
            {
                Status = "Success",
                Message = "Refresh Token Successful",
                Data = new JwtTokenReponseModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            });
        }

        [HttpPost("revoke"), Authorize]
        public async Task<IActionResult> Revoke()
        {
            var username = User.Identity.Name;
            var user = await _unitOfWork.UserRepo.FindAsync(u => u.Username == username);
            if (user == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = "Fail",
                    Message = "Bad Request",
                    Errors = new Dictionary<string, string>{
                        {"Username", "Username is not found"}
                    }
                });
            }
            var refreshKey = _configuration["JWT:RefreshKey"] + user.ID;
            await _cache.RemoveAsync(refreshKey);
            return NoContent();
        }
    }
}
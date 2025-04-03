using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.AuthDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using EPlatform_API.Services;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace EPlatform_API.Controllers.Identity
{
    [Route("api/v1/token")]
    public class TokenController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RedisServices _redisServices;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager; 
        private readonly ILogger<TokenController> _logger;
        private readonly IConfiguration _configuration;


        public TokenController(
            IUnitOfWork unitOfWork,
            ILogger<TokenController> logger,
            RedisServices redisServices,
            ITokenService tokenService,
            IConfiguration configuration,
            UserManager<AppUser> userManager
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _redisServices = redisServices;
            _tokenService = tokenService;
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody]JwtTokenRequestModel tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest(new ApiResponseStandard<JwtTokenRequestModel>
                {
                    Status = 400,
                    Message = "Not Found Token Model",
                });
            }
            var accessToken = tokenModel.AccessToken;
            var refreshToken = tokenModel.RefreshToken;

            if (accessToken == null){
                return BadRequest(new ApiResponseStandard<JwtTokenRequestModel>
                {
                    Status = 400,
                    Message = "Access Token doen not Exist",
                });
            }

            var principle = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userName = principle.Identity.Name;

            var user = await _userManager.FindByNameAsync(userName);
            var currentRefreshKey = _configuration["JWT:RefreshKey"] + user.Id;
            string currentRefreshToken = await _redisServices.GetString(currentRefreshKey);
            // if the refresh token is expired, the redis will delete it, Needless to check
            if (string.IsNullOrEmpty(currentRefreshToken))
            {
                return StatusCode(404, new ApiResponseStandard<JwtTokenRequestModel>
                {
                    Status = 404,
                    Message = "Not Found Refresh Token",
                });
            }

            if (user == null || currentRefreshToken != refreshToken)
            {
                return BadRequest(new ApiResponseStandard<JwtTokenRequestModel>
                {
                    Status = 400,
                    Message = "The Refresh Token is invalid"
                });
            }

            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _redisServices.SetString(
                currentRefreshKey,
                newRefreshToken,
                TimeSpan.FromDays(7)
            );

            return Ok(new ApiResponseStandard<JwtTokenReponseModel>
            {
                Status = 200,
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
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Username is not found"
                });
            }
            var refreshKey = _configuration["JWT:RefreshKey"] + user.Id;
            await _redisServices.RemoveString(refreshKey);
            return NoContent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.AuthDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using EPlatform_API.Services;
using EPlatform_API.Setting;
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

        // [HttpPost("refresh")]
        // public async Task<IActionResult> Refresh()
        // {
        //     var accessToken = HttpContext.Request.Cookies["access_Token"];
        //     var refreshToken = HttpContext.Request.Cookies["refresh_Token"];
        //     if (accessToken == null){
        //         return Unauthorized(new ApiResponseStandard<JwtTokenRequestModel>
        //         {
        //             Status = 401,
        //             Message = "Access Token doen not Exist",
        //         });
        //     }

        //     var principle = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        //     var userName = principle.Identity.Name;

        //     var user = await _userManager.FindByNameAsync(userName);
        //     var currentRefreshKey = _configuration["JWT:RefreshKey"] + user.Id;
        //     string currentRefreshToken = await _redisServices.GetString(currentRefreshKey);
        //     // if the refresh token is expired, the redis will delete it, Needless to check
        //     if (string.IsNullOrEmpty(currentRefreshToken))
        //     {
        //         return StatusCode(401, new ApiResponseStandard<JwtTokenRequestModel>
        //         {
        //             Status = 401,
        //             Message = "Not Found Refresh Token",
        //         });
        //     }

        //     if (user == null || currentRefreshToken != refreshToken)
        //     {
        //         return Unauthorized(new ApiResponseStandard<JwtTokenRequestModel>
        //         {
        //             Status = 401,
        //             Message = "The Refresh Token is invalid"
        //         });
        //     }

        //     var newAccessToken = await _tokenService.GenerateAccessToken(user);
        //     var newRefreshToken = _tokenService.GenerateRefreshToken();
        //     var jwtSetting = _configuration.GetSection("JWT").Get<JwtSetting>();

        //     await _redisServices.SetString(
        //         currentRefreshKey,
        //         newRefreshToken,
        //         TimeSpan.FromDays(jwtSetting.RefreshTokenExpiryDays)
        //     );

        //     await _tokenService.WriteTokenToCookie(newRefreshToken, newAccessToken, HttpContext, jwtSetting);

        //     return Ok(new ApiResponseStandard<JwtTokenReponseModel>
        //     {
        //         Status = 200,
        //         Message = "Refresh Token Successful"
        //     });
        // }

        [HttpPost("revoke"), Authorize]
        public async Task<IActionResult> Revoke()
        {
            try {
                var username = User.Identity.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new ApiResponseStandard<object>
                    {
                        Status = 401,
                        Message = "Username is not found"
                    });
                }
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return Unauthorized(new ApiResponseStandard<object>
                    {
                        Status = 400,
                        Message = "Username is not found"
                    });
                }
                var refreshKey = _configuration["JWT:RefreshKey"] + user.Id;
                await _redisServices.RemoveString(refreshKey);
                var cookieOption = new CookieOptions
                {
                    Path = "/", // This must match how it was originally set
                    Secure = true, // If the original cookie was Secure
                    HttpOnly = true, // Doesn’t affect deletion, but okay to set
                    SameSite = SameSiteMode.None // Match the original SameSite mode
                };
                HttpContext.Response.Cookies.Delete("access_token", cookieOption);
                HttpContext.Response.Cookies.Delete("refresh_token", cookieOption);
                return NoContent();
            } catch (Exception e) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Internal Server Error: " + e.Message
                });
            }
        }

        [HttpPost("check-authenticated-and-refresh"), Authorize]
        public async Task<IActionResult> IsAuthenticatedAndRefresh(){
            try {
                var accessToken = HttpContext.Request.Cookies["access_token"];
                if (string.IsNullOrEmpty(accessToken)){
                    return Unauthorized(new ApiResponseStandard<object>
                    {
                        Status = 401,
                        Message = "Access Token does not exist"
                    });
                }
                if (await _tokenService.IsTokenExpired(accessToken)){
                    try {
                        await _tokenService.RefreshExpiredToken(accessToken, HttpContext, _redisServices, _userManager);
                    } catch (Exception e) {
                        return Unauthorized(new ApiResponseStandard<object>
                        {
                            Status = 401,
                            Message = e.Message
                        });
                    }
                }
                return Ok(new ApiResponseStandard<bool>
                {
                    Status = 200,
                    Message = "Token is valid or refreshed",
                    Data = true
                });
            } catch (Exception e) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Internal Server Error: " + e.Message
                });
            }
        }
    
        [HttpPost("check-authorization")]
        [Authorize]
        public IActionResult CheckAuthorization([FromBody] RoleForCheckAuthorizeRequest request)
        {
            try {
                var user = User;
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    return Unauthorized(new ApiResponseStandard<object>
                    {
                        Status = 401,
                        Message = "User is not authenticated"
                    });
                }

                var isAuthorized = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Any(c => c.Value.Equals(request.Role, StringComparison.OrdinalIgnoreCase));

                return Ok(new ApiResponseStandard<bool>
                {
                    Status = 200,
                    Message = "User is authorized with role: " + request.Role,
                    Data = isAuthorized
                });
            } catch (Exception e) {
                return StatusCode(500, new ApiResponseStandard<object>
                {
                    Status = 500,
                    Message = "Internal Server Error: " + e.Message
                });
            }
        }
    }
}
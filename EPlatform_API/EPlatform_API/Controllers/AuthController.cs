using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.AuthDTOs;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Mappers;
using EPlatform_API.Models;
using EPlatform_API.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace EPlatform_API.Controllers
{
    [Route("/api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly DistributedCacheEntryOptions cacheOption;
        public AuthController(
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
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            cacheOption = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(7));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestModel loginModel){
            if (!ModelState.IsValid){
                return new BadRequestObjectResult(new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "The input is invalid"
                });
            }

            var user = await _unitOfWork.UserRepo.FindAsync(u => u.Username == loginModel.Username);
            if (user == null){
                return StatusCode(400, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Username doesn't exist",
                });
            }

            if (!_passwordHasher.Verify(user.PasswordHash,loginModel.Password)){
                return StatusCode(400, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Password is incorrect",
                });
            }

            // claim group
            var group = await GetGroupOfRole();

            if (group == null){
                return StatusCode(500, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 500,
                    Message = "The Group doesn't exist",
                });
            }

            var listClaims = new List<Claim>(){
                new Claim(ClaimTypes.Role, group.GroupName),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var userID = user.ID;
            JwtTokenReponseModel tokenResponse = new JwtTokenReponseModel();
            tokenResponse.AccessToken = _tokenService.GenerateAccessToken(listClaims);
            tokenResponse.RefreshToken = _tokenService.GenerateRefreshToken();
            await _cache.SetAsync<string>(
                _configuration["JWT:RefreshKey"]+user.ID,
                tokenResponse.RefreshToken,
                cacheOption
            );

            ApiResponseStandard<JwtTokenReponseModel> response = new ApiResponseStandard<JwtTokenReponseModel>(){
                Status = 200,
                Message = "Login Successful",
                Data = tokenResponse
            };

            return StatusCode(200, response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestModel registerModel){
            if (!ModelState.IsValid){
                return new BadRequestObjectResult(new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "The input is invalid"
                });
            }

            var user = await _unitOfWork.UserRepo.FindAsync(u => u.Username == registerModel.Username);
            if (user != null){
                return StatusCode(400, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Username existed in the system",
                });
            }

            if (registerModel.Password != registerModel.ConfirmPassword){
                return StatusCode(400, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Bad Request Error",
                });
            }

            user = registerModel.ToUser();
            
            var group = await GetGroupOfRole();
            
            if (group == null){
                return StatusCode(500, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 500,
                    Message = "The Group doesn't exist",
                });
            }

            user.GroupID = group.ID;
            user.PasswordHash = _passwordHasher.Hash(user.PasswordHash);
            await _unitOfWork.UserRepo.AddAsync(user);
            await _unitOfWork.SaveAsync();

            var listClaims = new List<Claim>(){
                new Claim(ClaimTypes.Role,group.GroupName),
                new Claim(ClaimTypes.Name,user.Username)
            };

            var userID = (await _unitOfWork.UserRepo.FindAsync(u => u.Username == user.Username))?.ID;
            JwtTokenReponseModel tokenResponse = new JwtTokenReponseModel();
            tokenResponse.AccessToken = _tokenService.GenerateAccessToken(listClaims);
            tokenResponse.RefreshToken = _tokenService.GenerateRefreshToken();
            await _cache.GetOrSetAsync<string>(
                _configuration["JWT:RefreshKey"]+user.ID,
                tokenResponse.RefreshToken,
                cacheOption
            );

            ApiResponseStandard<JwtTokenReponseModel> response = new ApiResponseStandard<JwtTokenReponseModel>(){
                Status = 201,
                Message = "Register User Successful",
                Data = tokenResponse
            };

            return StatusCode(201, response);
        }

        private async Task<Group> GetGroupOfRole(){
            return await _unitOfWork.GroupRepo.GetAllDataSet()
            .Include(g => g.GroupOfRoles)
            .ThenInclude(gOR => gOR.Role)
            .FirstOrDefaultAsync(gr => gr.GroupName.ToUpper() == "Customer".ToUpper());
        }
    }
}
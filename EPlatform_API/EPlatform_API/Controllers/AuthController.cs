using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
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
using Microsoft.AspNetCore.Authorization;
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
        private readonly ISendMailService _sendMailService;
        private readonly DistributedCacheEntryOptions cacheOption;
        public AuthController(
            IUnitOfWork unitOfWork, 
            ILogger<AuthController> logger, 
            IDistributedCache cache,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            ISendMailService sendMailService
        )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _sendMailService = sendMailService;
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

            var user = await _unitOfWork.UserRepo.GetAllDataSet()
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Username == loginModel.Username);
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
            var group = user.Group;

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
            // Check infor
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
            // Send Mail
            var OTP = GenerateOTP();
            await _sendMailService.SendEmailAsync(registerModel.Username,"OTP From TRANS to Sign Up",OTP);

            await _cache.SetAsync<string>($"OTP:{registerModel.Username}",OTP,new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

            return StatusCode(200, new ApiResponseStandard<object>{
                Status = 200,
                Message = "Please, Confirm The OTP"
            });
        }

        [HttpPost("register-confirm")]
        public async Task<IActionResult> ConfirmRegister([FromBody] RegisterRequestModel registerModel){
            if (!ModelState.IsValid){
                return BadRequest();
            }
            string OTP = "";
            _cache.TryGetValue<string>($"OTP:{registerModel.Username}",out OTP);
            _logger.LogCritical($"OTP: {OTP} -- ClientOTP: {registerModel.OTP}");
            if (OTP != registerModel.OTP){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "OTP is incorrect"
                });
            }
            
            var group = await GetGroupOfRole();
            
            if (group == null){
                return StatusCode(500, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 500,
                    Message = "The Group doesn't exist",
                });
            }

            Users user = registerModel.ToUser();

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
            return StatusCode(201,response);
        } 

        [HttpPost("reset-password"),Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto resetPasswordModel){
            if (!ModelState.IsValid){
                return BadRequest();
            }

            var username = User?.Identity?.Name;
            var user = await _unitOfWork.UserRepo.FindAsync(u => u.Username == username);
            
            if (user == null){
                return StatusCode(404, new ApiResponseStandard<object>{
                    Status = 404,
                    Message = "We don't found the user",
                    Timestamp = DateTime.Now
                });
            }
            
            if (resetPasswordModel.NewPassword != resetPasswordModel.ConfirmNewPassword){
                return StatusCode(400, new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The confirm passowrd has to equal with new password",
                    Timestamp = DateTime.Now
                });
            }

            if (!_passwordHasher.Verify(user.PasswordHash,resetPasswordModel.OldPassword)){
                return StatusCode(400, new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The old passowrd is incorrect",
                    Timestamp = DateTime.Now
                });
            }

            user.PasswordHash = _passwordHasher.Hash(resetPasswordModel.NewPassword);
            _unitOfWork.UserRepo.Update(user);
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
            
            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Updated Successful!",
                Timestamp = DateTime.Now
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestModel forgotPasswordModel){
            if (!ModelState.IsValid){
                return BadRequest();
            }

            var user = await _unitOfWork.UserRepo.FindAsync(u => u.Email == forgotPasswordModel.Email);
            if (user == null){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The email does not exist"
                });
            }
            var OTP = GenerateOTP();
            await _sendMailService.SendEmailAsync(user.Email,"OTP From TRANS to Recovery Password",OTP);
            await _cache.SetAsync<string>($"OTP:{user.Email}",OTP,new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Please, Verify OTP"
            });
        }
        [HttpPost("confirm-recovery-otp")]
        public async Task<IActionResult> ConfirmRecoveryOTP(ForgotPasswordRequestModel forgotPasswordModel){
            if (!ModelState.IsValid){
                return BadRequest();
            }

            var user = await _unitOfWork.UserRepo.GetAllDataSet()
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Email == forgotPasswordModel.Email);

            if (user == null){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The email doesn't exist"
                });
            }
            var OTP = "";
            _cache.TryGetValue<string>($"OTP:{user.Username}", out OTP);
            if (OTP != forgotPasswordModel.OTP){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "OTP is incorrect",
                    Timestamp = DateTime.Now
                });
            }

            var claims = new List<Claim>{
                new Claim(ClaimTypes.Role,user.Group.GroupName),
                new Claim(ClaimTypes.Name,user.Username)
            };

            var jwtToken = new JwtTokenReponseModel{
                AccessToken = _tokenService.GenerateAccessToken(claims),
                RefreshToken = _tokenService.GenerateRefreshToken()
            };
            await _cache.SetAsync<string>(
                _configuration["JWT:RefreshKey"],
                jwtToken.RefreshToken,
                cacheOption
            );

            return Ok(new ApiResponseStandard<JwtTokenReponseModel>{
                Status = 200,
                Data = jwtToken,
                Message = "Recovery Password Successful!"
            });

        }
        private async Task<Group> GetGroupOfRole(string grName = "Customer"){
            return await _unitOfWork.GroupRepo.GetAllDataSet()
            .Include(g => g.GroupOfRoles)
            .ThenInclude(gOR => gOR.Role)
            .FirstOrDefaultAsync(gr => gr.GroupName == grName);
        }
    
        private string GenerateOTP(int length=5){
            using (var rng = RandomNumberGenerator.Create()){
                var bytes = new byte[length/2];
                rng.GetBytes(bytes);
                
                var OTP = new StringBuilder();
                
                foreach (var b in bytes){
                    OTP.Append(((int)b%10).ToString());
                }

                while (OTP.Length < 5){
                    rng.GetBytes(bytes);
                    OTP.Append(((int)bytes[0]%10).ToString());
                }
                return OTP.ToString();
            }
        }
    }
}
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace EPlatform_API.Controllers.Identity
{
    [Route("/api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IDistributedCache _cache;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ISendMailService _sendMailService;
        private readonly DistributedCacheEntryOptions cacheOption;
        private readonly AppDbContext _dbContext;
        public AuthController(
            ILogger<AuthController> logger, 
            IDistributedCache cache,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            ISendMailService sendMailService,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext appDbContext
        )
        {
            _logger = logger;
            _cache = cache;
            _tokenService = tokenService;
            _configuration = configuration;
            _sendMailService = sendMailService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext = appDbContext;
            cacheOption = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(7));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginModel){
            if (!ModelState.IsValid){
                return new BadRequestObjectResult(new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "The input is invalid"
                });
            }

            if (loginModel == null){
                return new BadRequestObjectResult(new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Login Information is empty"
                }); 
            }

            var user = await _userManager.FindByNameAsync(loginModel.Username);
            
            if (user == null){
                return StatusCode(400, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Username is not exist in the system",
                });
            }

            if (await _userManager.IsLockedOutAsync(user)){
                return StatusCode(400, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Your account is locked",
                });
            }

            var result = await _signInManager.PasswordSignInAsync(userName: loginModel.Username,password: loginModel.Password,isPersistent: false, lockoutOnFailure: true);      
            
            if (!result.Succeeded){
                return StatusCode(400, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Password is incorrect",
                });
            }
            
            JwtTokenReponseModel tokenResponse = new JwtTokenReponseModel();
            tokenResponse.AccessToken = await _tokenService.GenerateAccessToken(user);
            tokenResponse.RefreshToken = _tokenService.GenerateRefreshToken();
            await _cache.SetAsync<string>(
                _configuration["JWT:RefreshKey"]+user.Id,
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

            var user = await _userManager.FindByNameAsync(registerModel.Username);
            if (user != null){
                return StatusCode(400, new ApiResponseStandard<JwtTokenReponseModel>{
                    Status = 400,
                    Message = "Username existed in the system",
                });
            }

            // Send Mail
            var verifyCode = GenerateVerifyCode();
            await _sendMailService.SendEmailAsync(registerModel.Username,"OTP From TRANS to Sign Up",$"<h3>Verify Code is: {verifyCode}</h3>");

            await _cache.SetAsync<string>($"VerifyCode:{registerModel.Username}",verifyCode,new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

            return StatusCode(200, new ApiResponseStandard<object>{
                Status = 200,
                Message = "Please, Confirm The Verify Code"
            });
        }

        [HttpPost("register-confirm")]
        public async Task<IActionResult> ConfirmRegister([FromBody] RegisterRequestModel registerModel){
            if (!ModelState.IsValid){
                return BadRequest();
            }
            string verifyCode = "";
            _cache.TryGetValue<string>($"VerifyCode:{registerModel.Username}",out verifyCode);
            _logger.LogCritical($"VerifyCode: {verifyCode} -- ClientOTP: {registerModel.VerifyCode}");
            if (verifyCode != registerModel.VerifyCode){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "OTP is incorrect"
                });
            }


            var user = registerModel.ToUser();

            using (var transaction = await _dbContext.Database.BeginTransactionAsync()){
                var createResult = await _userManager.CreateAsync(user,registerModel.Password);
                if (!createResult.Succeeded){
                    StringBuilder str = new StringBuilder();
                    foreach (var err in createResult.Errors){
                        str.Append($"{err.Description} \n");
                    }
                    transaction.Rollback();
                    return StatusCode(500,new ApiResponseStandard<object>{
                        Status = 500,
                        Message = str.ToString()
                    }); 
                }

                var result = await _userManager.AddToRoleAsync(user,"Customer");
                if (!result.Succeeded){
                    transaction.Rollback();
                    return StatusCode(500,new ApiResponseStandard<object>{
                        Status = 500,
                        Message = "Customer Role doesn't exist"
                    });
                }

                var listClaims = new List<Claim>{
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "Customer")
                };

                var userExist = await _userManager.FindByNameAsync(user.UserName);

                var addClaimResult = await _userManager.AddClaimsAsync(userExist, listClaims);

                if (!addClaimResult.Succeeded){
                    transaction.Rollback();
                    return StatusCode(400,new ApiResponseStandard<object>{
                        Status = 400,
                        Message = "Claim is in valid"
                    });
                }

                JwtTokenReponseModel tokenResponse = new JwtTokenReponseModel();
                tokenResponse.AccessToken = await _tokenService.GenerateAccessToken(userExist);
                tokenResponse.RefreshToken = _tokenService.GenerateRefreshToken();
                await _cache.GetOrSetAsync<string>(
                    _configuration["JWT:RefreshKey"]+user.Id,
                    tokenResponse.RefreshToken,
                    cacheOption
                );

                ApiResponseStandard<JwtTokenReponseModel> response = new ApiResponseStandard<JwtTokenReponseModel>(){
                    Status = 201,
                    Message = "Register User Successful",
                    Data = tokenResponse
                };

                transaction.Commit();

                return StatusCode(201,response);
            }
        } 

        [HttpPost("reset-password"),Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto resetPasswordModel){
            if (!ModelState.IsValid){
                return BadRequest();
            }

            var username = User?.Identity?.Name;
            var user = await _userManager.FindByNameAsync(username);
            
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

            var checkResult = await _userManager.CheckPasswordAsync(user,resetPasswordModel.OldPassword);

            if (!checkResult){
                return StatusCode(400, new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The old passowrd is incorrect",
                    Timestamp = DateTime.Now
                });
            }

            var changeResult = await _userManager.ChangePasswordAsync(user,resetPasswordModel.OldPassword,resetPasswordModel.NewPassword);
            
            if (!changeResult.Succeeded){
                return StatusCode(500, new ApiResponseStandard<object>{
                    Status = 500,
                    Message = "Something wrong happen",
                    Timestamp = DateTime.Now
                });
            }

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

            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The email does not exist"
                });
            }
            var verifyCode = GenerateVerifyCode();
            await _sendMailService.SendEmailAsync(user.Email,"Verify Code From TRANS To Recovery Account",$"<h3>The verify code is: {verifyCode}</h3>");
            await _cache.SetAsync<string>($"AuthVerifyCode:{user.Email}",verifyCode,new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Please, Verify To Recovery Account"
            });
        }
        [HttpPost("confirm-recovery-verifycode")]
        public async Task<IActionResult> ConfirmRecoveryOTP(ForgotPasswordRequestModel forgotPasswordModel){
            if (!ModelState.IsValid){
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

            if (user == null){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The email doesn't exist"
                });
            }
            var verifyCode = "";
            _cache.TryGetValue<string>($"AuthVerifyCode:{user.UserName}", out verifyCode);
            if (verifyCode != forgotPasswordModel.VerifyCode){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "Verify Code is incorrect",
                    Timestamp = DateTime.Now
                });
            }

            var jwtToken = new JwtTokenReponseModel{
                AccessToken = await _tokenService.GenerateAccessToken(user),
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
    
        private string GenerateVerifyCode(int length=6){
            using (var rng = RandomNumberGenerator.Create()){
                var bytes = new byte[length/2];
                rng.GetBytes(bytes);
                
                var verifyCode = new StringBuilder();
                
                foreach (var b in bytes){
                    verifyCode.Append(((int)b%10).ToString());
                }

                while (verifyCode.Length < length){
                    rng.GetBytes(bytes);
                    verifyCode.Append(((int)bytes[0]%10).ToString());
                }
                return verifyCode.ToString();
            }
        }
    }
}
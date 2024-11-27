using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.AdminDTOs.Roles;
using EPlatform_API.DTOs.AdminDTOs.Users;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.DTOs.AuthDTOs.Users;
using EPlatform_API.Helper;
using EPlatform_API.Mappers;
using EPlatform_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EPlatform_API.Controllers.Identity
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Policy = "UserManagePolicy")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext dbContext,
            ILogger<UserController> logger
        )
        {
            _userManager = userManager;
            _context = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryStringModel queryString)
        {
            var usersQueryable = _userManager.Users;
            
            if (!queryString.SearchString.IsNullOrEmpty()){
                usersQueryable = usersQueryable.Where(u => u.UserName.Contains(queryString.SearchString));
            }       
            
            var users = PageList<AppUser>.ToPageList(usersQueryable, queryString.PageNumber, queryString.PageSize);
            users.AddPagingInfoToHeader(Response);
            var usersApiResponse = new List<object>();
            foreach (var user in users){
                var listRole = _context.UserRoles
                .Where(u => u.UserId == user.Id)
                .Select(u => u.RoleId);
                
                var roles = await _context.Roles.Where(r => listRole.Contains(r.Id))
                .Select(r => r.Name).ToListAsync();
                
                var userResObject = new {
                    Id = user.Id,
                    Username = user.UserName,
                    Created = user.Create,
                    Roles = roles,
                    Active = (user.LockoutEnd == null) ? true : (user.LockoutEnd < DateTime.Now) ? true : false
                };

                usersApiResponse.Add(userResObject);
            }

            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Get all user",
                Data = usersApiResponse
            });
        }
    
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserRequestModel createUserModel){
            if (!ModelState.IsValid){
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(createUserModel.Username);
            if (user != null){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "User has existed in the system"
                });
            }

            if (createUserModel.Password != createUserModel.ConfirmPassword){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "confirm password is incorrect"
                });
            }
            
            var newUser = createUserModel.ToUser();

            using (var transaction = _context.Database.BeginTransaction()){
                var createResult = await _userManager.CreateAsync(newUser);
                if (!createResult.Succeeded){
                    transaction.Rollback();
                    return StatusCode(500,(new ApiResponseStandard<object>{
                        Status = 500,
                        Message = "This service is occurred some error, please try again"
                    }));
                }

                var newUserInDB = await _userManager.FindByNameAsync(newUser.UserName);
                var addRoleResult = await _userManager.AddToRoleAsync(newUserInDB, "Customer");
                
                if (!addRoleResult.Succeeded){
                    _logger.LogError("In UserController - CreateUser Action: Customer Role may not have been initialized yet!");
                    transaction.Rollback();
                    return StatusCode(500,new ApiResponseStandard<object>{
                        Status = 500,
                        Message = "This service is occurred some error, please try again"
                    });
                }

                transaction.Commit();
                return StatusCode(201, new ApiResponseStandard<string>{
                    Status = 201,
                    Message = "Create Successful",
                    Data = newUser.UserName
                });
                }

        }
    
        [HttpPost("lock-or-unlock-user")]
        public async Task<IActionResult> LockOrUnlockUser([FromBody] LockOrUnlockUserRequestModel lockModel){
            var user = await _userManager.FindByIdAsync(lockModel.Id);

            if (user == null){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The user is not found"
                });
            }

            bool isActive = (user.LockoutEnd == null) ? true : (user.LockoutEnd < DateTime.Now) ? true : false;
            
            var result = new IdentityResult();

            // If account isn't locked, lock it and else
            if (isActive){
                result = await _userManager.SetLockoutEndDateAsync(user,DateTime.MaxValue);
            } else {
                result = await _userManager.SetLockoutEndDateAsync(user,null);
            }

            if (!result.Succeeded){
                return StatusCode(500,(new ApiResponseStandard<object>{
                    Status = 500,
                    Message = "This service is occurred some error, please try again"
                }));
            }
            
            return Ok(new ApiResponseStandard<bool>{
                Status = 200,
                Message = "The user status is update",
                Data = !isActive
            });
        }
    
        [HttpPost("setting-role-for-user")]
        public async Task<IActionResult> SettingRoleForUser([FromBody] SettingRoleRequestModel settingRoleModel){
            if (!await IsSettingRoleParameterValid(settingRoleModel.rolesId)){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "Parameter is have the role not exist in the system"
                });
            }
            
            var userRole = _context.UserRoles.Where(ur => ur.UserId == settingRoleModel.Id).Select(ur => ur.RoleId).AsNoTracking();

            var removeList = userRole.Where(rm => !settingRoleModel.rolesId.Contains(rm));
            var addList = settingRoleModel.rolesId.Where(a => !userRole.Contains(a));

            var removeListObject = _context.UserRoles.Where(rm => removeList.Contains(rm.RoleId) && rm.UserId == settingRoleModel.Id);
            var addListObject = new List<IdentityUserRole<string>>();

            foreach (var ur in addList)
            {
                addListObject.Add(new IdentityUserRole<string>{
                    RoleId = ur,
                    UserId = settingRoleModel.Id
                });
            }

            _context.UserRoles.RemoveRange(removeListObject);
            await _context.UserRoles.AddRangeAsync(addListObject);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseStandard<object>{
                Status = 200,
                Message = "Setting Role Successful"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id){
            var user = await _userManager.FindByIdAsync(id);

            if (user == null){
                return BadRequest(new ApiResponseStandard<object>{
                    Status = 400,
                    Message = "The user is not found"
                });
            }

            var userResponse = user.ToUserDetailResponse();
            
            var userRoles = await _userManager.GetRolesAsync(user);
            userResponse.Roles = (List<string>)userRoles;
            return Ok(new ApiResponseStandard<UserDetailResponseModel>{
                Status = 200,
                Message = "Access Successful!",
                Data = userResponse,
                Resources = new List<Link>{
                    {
                        new Link{
                            _Link = "/admin/users/{id}/set-role",
                            Method = "POST"
                        }
                    }
                },
                Timestamp = DateTime.Now
            });
        }


        private async Task<bool> IsSettingRoleParameterValid(List<string> rolesParameter){
            var roles = await _context.Roles.Select(r => r.Name).ToListAsync();
            foreach (var role in rolesParameter)
            {
                if (!roles.Contains(role)){
                    return false;
                }
            }
            return true;
        }
    }
}
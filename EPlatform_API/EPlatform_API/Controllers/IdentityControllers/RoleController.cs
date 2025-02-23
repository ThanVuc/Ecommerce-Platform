using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EPlatform_API.Data;
using EPlatform_API.DTOs.AdminDTOs.Roles;
using EPlatform_API.DTOs.ApiStandard;
using EPlatform_API.Helper;
using EPlatform_API.IServices;
using EPlatform_API.Mappers;
using EPlatform_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EPlatform_API.Controllers.Identity
{
    [Route("api/v1/admin")]
    [ApiController]
    [Authorize(Policy = "RoleManagePolicy")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILoggingService _loggingSVC;
        public RoleController(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager,
            AppDbContext appDbContext,
            ILoggingService loggingService
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = appDbContext;
            _loggingSVC = loggingService;
        }

        [HttpGet("role-layout"), Authorize]
        public async Task<IActionResult> GetRolesLayout()
        {
            var username = User?.Identity?.Name;
            if (username == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found user, please try login again"
                });
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found user, please try login again(1)"
                });
            }

            var responseAPI = new ApiResponseStandard<string>
            {
                Message = "Get Layout Information",
                Data = user.Last,
                Status = 200,
                Resources = new List<Link>{
                    new Link{
                        _Link = @"http://localhost:5119/images/test_avt.jpg",
                        Method = "GET"
                    },
                    new Link{
                        _Link = "/admin/users",
                        Method = "GET"
                    },
                    new Link{
                        _Link = "/admin/roles",
                        Method = "GET"
                    },
                    new Link{
                        _Link = "",
                        Method = "GET"
                    }
                }
            };
            return Ok(responseAPI);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles([FromQuery] RoleQueryStringModel queryString)
        {
            var roles = PageList<IdentityRole>.ToPageList(_roleManager.Roles, queryString.PageNumber, queryString.PageSize);
            roles.AddPagingInfoToHeader(Response);
            List<object> roleResponsesData = new List<object>();
            foreach (var role in roles)
            {
                var claims = await _context.RoleClaims.Where(c => c.RoleId == role.Id).ToListAsync();
                List<ClaimResponseModel> claimsOfRole = new List<ClaimResponseModel>();
                foreach (var claim in claims)
                {
                    claimsOfRole.Add(new ClaimResponseModel
                    {
                        ClaimId = claim.Id,
                        ClaimType = claim.ClaimType,
                        ClaimValue = claim.ClaimValue
                    });
                }
                roleResponsesData.Add(new
                {
                    roleId = role.Id,
                    roleName = role.Name,
                    claims = claimsOfRole
                });
            }

            var apiResponse = new ApiResponseStandard<List<object>>()
            {
                Status = 200,
                Message = "Get All Role",
                Data = roleResponsesData,
                Resources = new List<Link>{
                    new Link{
                        _Link = "admin/roles/create",
                        Method = "POST"
                    },
                    new Link{
                        _Link = "admin/roles/edit/{id}",
                        Method = "PUT"
                    },
                    new Link{
                        _Link = "admin/roles/delete/{id}",
                        Method = "Delete"
                    }
                }
            };
            return Ok(apiResponse);
        }

        [HttpPost("roles/add")]
        public async Task<IActionResult> AddRole([FromBody] CreateRoleRequestModel createRoleModel)
        {
            var isExist = await _roleManager.RoleExistsAsync(createRoleModel.RoleName);

            if (isExist)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Role is Exist",
                    Timestamp = DateTime.Now
                });
            }

            var newRole = new IdentityRole(createRoleModel.RoleName);
            var addResult = await _roleManager.CreateAsync(newRole);

            if (!addResult.Succeeded)
            {
                var errString = GetErrorFromResult(addResult.Errors);
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = errString,
                    Timestamp = DateTime.Now
                });
            }

            await _loggingSVC.WriteRoleLog(@$"admin:{User.Identity.Name}---add-role---role:{createRoleModel.RoleName}---time:{DateTime.Now}");

            return StatusCode(201, new ApiResponseStandard<string>
            {
                Status = 201,
                Message = "Create Role Success",
                Data = createRoleModel.RoleName
            });
        }

        [HttpDelete("roles/delete/{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute] string id)
        {
            var deleteRole = await _roleManager.FindByIdAsync(id);
            if (deleteRole == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found the role"
                });
            }

            var deleteResult = await _roleManager.DeleteAsync(deleteRole);
            if (!deleteResult.Succeeded)
            {
                var errString = GetErrorFromResult(deleteResult.Errors);
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = errString
                });
            }

            await _loggingSVC.WriteRoleLog(@$"admin:{User.Identity.Name}---delete-role---role:{deleteRole.Name}---time:{DateTime.Now}");


            return Ok(new ApiResponseStandard<string>
            {
                Status = 200,
                Message = "Delete Role Successful",
                Data = deleteRole.Name
            });
        }

        [HttpPut("roles/update/{id}")]
        public async Task<IActionResult> UpdateRole([FromRoute] string id ,[FromBody] UpdateRoleRequestModel updateRoleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var updateRole = await _roleManager.FindByIdAsync(id);

            if (updateRole == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found the role"
                });
            }

            updateRole.Name = updateRoleModel.Name;

            var updateResult = await _roleManager.UpdateAsync(updateRole);
            if (!updateResult.Succeeded)
            {
                var errString = GetErrorFromResult(updateResult.Errors);
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = errString
                });
            }

            await _loggingSVC.WriteRoleLog(@$"admin:{User.Identity.Name}---update-role---role:{updateRole.Name}---time:{DateTime.Now}");


            return Ok(new ApiResponseStandard<string>
            {
                Status = 200,
                Message = "Update Role Successful",
                Data = updateRole.Name
            });

        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRoleById([FromRoute] string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Role doesn't exist"
                });
            }

            var claims = await _context.RoleClaims
            .Where(c => c.RoleId == role.Id)
            .ToListAsync();

            var claimsDataResponse = new List<ClaimResponseModel>();

            foreach (var claim in claims)
            {
                claimsDataResponse.Add(new ClaimResponseModel
                {
                    ClaimId = claim.Id,
                    ClaimType = claim.ClaimType,
                    ClaimValue = claim.ClaimValue
                });
            }

            var dataResponse = new
            {
                roleName = role.Name,
                claims = claimsDataResponse
            };

            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Data = dataResponse,
                Message = "Get Detail of Role Successful",
                Resources = new List<Link>{
                    {
                        new Link{
                            _Link = "",
                            Method = "POST"
                        }
                    },
                    {
                        new Link{
                            _Link = "",
                            Method = "PUT"
                        }
                    },
                    {
                        new Link{
                            _Link = "",
                            Method = "DELETE"
                        }
                    }
                }
            });

        }

        [HttpPost("roles/{id}/add-claim")]
        public async Task<IActionResult> AddNewClaim([FromRoute] string id, [FromBody] AddNewClaimRequestModel addClaimModel)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found the role"
                });
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            var isDuplicate = claims.FirstOrDefault(c => c.Type == addClaimModel.ClaimType && c.Value == addClaimModel.ClaimValue);
            if (isDuplicate != null){
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Claim is exist in this role"
                });
            }

            var claim = new Claim(addClaimModel.ClaimType, addClaimModel.ClaimValue);
            var addResult = await _roleManager.AddClaimAsync(role, claim);

            if (!addResult.Succeeded)
            {
                var errStr = GetErrorFromResult(addResult.Errors);
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = errStr
                });
            }

            await _loggingSVC.WriteRoleLog(@$"admin:{User.Identity.Name}---add-role-claim---role:{role.Name}---claim:{addClaimModel.ClaimType}---time:{DateTime.Now}");

            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Add Claim Successful!"
            });
        }

        [HttpDelete("roles/{roleId}/delete-claim/{claimId}")]
        public async Task<IActionResult> DeleteClaim([FromRoute] string roleId,[FromRoute] int claimId )
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found the role"
                });
            }

            var claim = await _context.RoleClaims
            .FirstOrDefaultAsync(c => c.Id == claimId);

            if (claim == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found the claim"
                });
            }

            _context.RoleClaims.Remove(claim);
            await _context.SaveChangesAsync();

            await _loggingSVC.WriteRoleLog(@$"admin:{User.Identity.Name}---dlete-role-claim---role:{role.Name}---claim:{claim.ClaimType}---time:{DateTime.Now}");

            return Ok(new ApiResponseStandard<object>
            {
                Status = 200,
                Message = "Delete the claim successful"
            });
        }

        [HttpPut("roles/{roleId}/update-claim/{claimId}")]
        public async Task<IActionResult> UpdateClaim([FromRoute] string roleId,[FromRoute] int claimId, [FromBody] UpdateClaimRequestModel updateClaimModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found the role"
                });
            }

            var claim = await _context.RoleClaims.FirstOrDefaultAsync(c => c.Id == claimId);

            if (claim == null)
            {
                return BadRequest(new ApiResponseStandard<object>
                {
                    Status = 400,
                    Message = "Not found the claim"
                });
            }

            claim.ClaimType = updateClaimModel.ClaimType;
            claim.ClaimValue = updateClaimModel.ClaimValue;

            await _context.SaveChangesAsync();

            await _loggingSVC.WriteRoleLog(@$"admin:{User.Identity.Name}---update-role-claim---role:{role.Name}---claim:{claim.ClaimType}---time:{DateTime.Now}");
            return Ok(new ApiResponseStandard<string>
            {
                Status = 200,
                Message = "Update Claim Successful!"
            });
        }

        private string GetErrorFromResult(IEnumerable<IdentityError> errors)
        {
            StringBuilder errString = new StringBuilder();
            foreach (var err in errors)
            {
                errString.Append(err.Description + "\n");
            }
            return errString.ToString();
        }
    }
}
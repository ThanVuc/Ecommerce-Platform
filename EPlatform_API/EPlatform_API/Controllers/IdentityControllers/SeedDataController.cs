using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.IServices;
using Microsoft.AspNetCore.Mvc;

namespace EPlatform_API.Controllers.IdentityControllers
{
    [ApiController]
    [Route("api/seed")]
    public class SeedDataController : ControllerBase
    {
        private readonly ISeedDataService _seedDataService;

        public SeedDataController(ISeedDataService seedDataService)
        {
            _seedDataService = seedDataService; 
        }

        [HttpGet("users")]
        public async Task<IActionResult> SeedUser(){
            await _seedDataService.SeedUserData();
            return Ok("Seed Data Successful!");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.IRepository;
using EPlatform_API.Models.ShopOwners;
using EPlatform_API.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace EPlatform_API.Controllers.ShopOwnerControllers
{
    [ApiController]
    [Route("api/v1/shops/{shopId}/customer-supports")]
    public class CustomerSupportController : ControllerBase
    {
        private readonly ChatRepository _chatRepo;

        public CustomerSupportController(IMongoDatabase mongoDatabase)
        {
            _chatRepo = new ChatRepository(mongoDatabase);
        }

        [HttpGet("/api/chat/get-char-in-today-of-user")]
        public async Task<IActionResult> GetChatInTodayOfUser([FromQuery] string userId){
            var chats = await _chatRepo.GetChatTodayOfUser(userId);
            return Ok(chats);
        }
    }
}
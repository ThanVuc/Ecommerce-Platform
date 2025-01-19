using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models.ShopOwners;

namespace EPlatform_API.IRepository
{
    public interface IChatRepository
    {
        Task<List<Chat>> GetChatTodayOfUser(string userId);
    }
}
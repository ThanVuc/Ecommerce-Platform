using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models;

namespace EPlatform_API.IServices
{
    public interface IQueryingServices
    {
        public Task<IQueryable<AppUser>?> SearchUserAsync(string searchTerm);
        public Task<List<string>?> GetUserSuggestionAsync(string searchTerm);
    }
}
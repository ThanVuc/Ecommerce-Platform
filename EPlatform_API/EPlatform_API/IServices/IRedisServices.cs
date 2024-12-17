using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.IServices
{
    public interface IRedisServices
    {
        public Task IncreaseSearchTermCount(string searchTerm);
    }
}
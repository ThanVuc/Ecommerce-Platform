using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.IServices
{
    public interface ILoggingService
    {
        Task WriteAccountLog(string message);
        Task WriteRoleLog(string message);
    }
}
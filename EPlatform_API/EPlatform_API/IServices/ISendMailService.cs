using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.IServices
{
    public interface ISendMailService
    {
        Task SendEmailAsync(string email, string subject, string htmlContent);
    }
}
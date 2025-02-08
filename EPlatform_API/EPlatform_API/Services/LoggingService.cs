using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.IServices;
using MimeKit;

namespace EPlatform_API.Services
{
    public class LoggingService : ILoggingService
    {
        public async Task WriteAccountLog(string message)
        {
            Directory.CreateDirectory("LogFolder");
            var path = @$"LogFolder/account-log.txt";
            
            await WriteLogToFile(path, message);
        }

        public async Task WriteProductLog(string message)
        {
            Directory.CreateDirectory("LogFolder");
            var path = @$"LogFolder/product-log.txt";
            message = $"{DateTime.Now} - {message}";
            await WriteLogToFile(path, message);
        }

        public async Task WriteRoleLog(string message)
        {
            Directory.CreateDirectory("LogFolder");
            var path = @$"LogFolder/role-log.txt";

            await WriteLogToFile(path, message);
        }

        async Task WriteLogToFile(string path, string message){
            using (StreamWriter writer = new StreamWriter(path,true)){
                await writer.WriteAsync(message+"\n");
            }
        }
    }
}
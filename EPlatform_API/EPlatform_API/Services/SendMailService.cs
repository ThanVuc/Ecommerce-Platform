using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using MimeKit;

namespace EPlatform_API.Services
{
    public class SendMailService : ISendMailService
    {
        private MailModel mailModel;
        private readonly IConfiguration _configuration;
        public SendMailService(IConfiguration configuration){
            mailModel = new MailModel();
            _configuration = configuration;
            mailModel.Mail = _configuration["MailConfig:Mail"];
            mailModel.DisplayName = _configuration["MailConfig:DisplayName"];
            mailModel.Password = _configuration["MailConfig:Password"];
            mailModel.Host = _configuration["MailConfig:Host"];
            mailModel.Port = int.Parse(_configuration["MailConfig:Port"]);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlContent)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailModel.DisplayName, mailModel.Mail);
            message.From.Add(new MailboxAddress(mailModel.DisplayName, mailModel.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = htmlContent;
            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try{
                smtp.Connect(mailModel.Host, mailModel.Port, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(mailModel.Mail,mailModel.Password);
                await smtp.SendAsync(message);
            } catch (Exception ex){
                System.IO.Directory.CreateDirectory("mailsave");
                var emailFile = @$"mailsave/{Guid.NewGuid()}.eml";
                await message.WriteToAsync(emailFile);
            }

            smtp.Disconnect(true);
        }
    }
}
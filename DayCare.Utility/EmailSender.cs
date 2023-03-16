using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayCare.Utility
{
    public class EmailSender: IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration.GetSection("EmailConfigurations");
        }

        public Task SendEmailAsync(string To, string subject, string htmlMessage) {
            var Username = _configuration.GetValue<string>("Username");
            var Password = _configuration.GetValue<string>("Password");
            var Host = _configuration.GetValue<string>("Host");
            var Port = _configuration.GetValue<int>("Port");
            var From = _configuration.GetValue<string>("From");
            
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(From));
            email.To.Add(MailboxAddress.Parse(To));
            email.Subject = subject;
            email.Body = new TextPart(htmlMessage)
            {
                Text = htmlMessage,
            };

            using var smtp = new SmtpClient();
            smtp.Connect(Host, Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(Username, Password);
            smtp.Send(email);
            smtp.Disconnect(true);
            return Task.CompletedTask;
        }


    }
}

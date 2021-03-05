using SendGrid;
using SendGrid.Helpers.Mail;
using SquashLeague.Application.Contracts;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace SquashLeague.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        
        public async Task<bool> SendEmail(string emailAddress, string mailContent)
        {
            
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("doomedmorale80@gmail.com", "Liga squasha");
            var subject = "Potwierdzenie rejestracji w lidze squasha";
            var to = new EmailAddress(email: emailAddress);
            var message = MailHelper.CreateSingleEmail(from, to, subject, "AAAAAA", mailContent);
            var result = await client.SendEmailAsync(message);

            
            

            return true;
        }


    }
}

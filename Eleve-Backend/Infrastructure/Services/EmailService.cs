using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Eleve_Backend.Application.Interfaces;

namespace Eleve_Backend.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string to,string subject, string body)
        {
            var settings = _configuration.GetSection("EmailSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Eleve Store", settings["Sender"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject=subject;

            var bodyBuilder=new BodyBuilder { HtmlBody = body };
            email.Body=bodyBuilder.ToMessageBody();

            using var smtp=new SmtpClient();

            try
            {
                // Connect to Mailtrap/SMTP Server
                await smtp.ConnectAsync(settings["Host"], 
                    int.Parse(settings["Port"]), 
                    MailKit.Security.SecureSocketOptions.Auto);

                // Authenticate using the Username (e.g., "api" or Mailtrap ID)
                await smtp.AuthenticateAsync(settings["Username"], settings["Password"]);
                
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                
                return true; // Success
            }
            catch (Exception ex)
            {
                // Log the error (You can inject ILogger if you have it)
                Console.WriteLine($"Email failed: {ex.Message}");
                return false; // Failed
            }
        }
    }
}

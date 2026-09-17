using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace FoodDeliverySystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail,
                                         string subject,
                                         string body)
        {
            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = _configuration["EmailSettings:Host"],
                    Port = int.Parse(_configuration["EmailSettings:Port"]),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                      _configuration["EmailSettings:Email"],
                      _configuration["EmailSettings:Password"])
                };

                var message = new MailMessage
                {
                    From = new MailAddress(
                        _configuration["EmailSettings:Email"],
                        "Food Xpress"),

                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                await smtpClient.SendMailAsync(message);
            }
            catch
            {
                throw new InvalidOperationException(
                    "Unable to send the verification email. Please check your internet connection and try again.");
            }
        }
    }
}
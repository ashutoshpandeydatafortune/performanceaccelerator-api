using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DF_EvolutionAPI.Utils;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace DF_EvolutionAPI.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration, IOptions<EmailService> options)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmail(string toEmail, string subject, string htmlContent)
        {
            
            try
            {
                using var smtpClient = new SmtpClient(Constant.SMTP_HOST, Constant.SMTP_PORT)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(Constant.SMTP_USERNAME, Constant.SMTP_PASSWORD)
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(Constant.SMTP_USERNAME),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                //mail.To.Add(new MailAddress(toEmail));              
                mail.To.Add(toEmail);              

                await smtpClient.SendMailAsync(mail);               
            }
            catch (Exception ex)
            {
                 throw new Exception($"Failed to send email: {ex.Message}");
            }

            return true;
        }
    }
}


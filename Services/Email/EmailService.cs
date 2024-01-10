using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;

namespace DF_EvolutionAPI.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<EmailService> _options;
        public EmailService(IConfiguration configuration, IOptions<EmailService> options)
        {
            _configuration = configuration;
            _options = options;
        }

        public async Task<ResponseModel> SendEmail(string toEmail, string subject, string htmlContent)
        {
            ResponseModel model = new ResponseModel();
            var smtpHost = _configuration.GetValue<string>("Mail:SMTP_HOST");
            var smtpPort = _configuration.GetValue<int>("Mail:SMTP_PORT");
            var smtpUsername = _configuration.GetValue<string>("Mail:SMTP_USERNAME");
            var smtpPassword = _configuration.GetValue<string>("Mail:SMTP_PASSWORD");

            try
            {
                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {

                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(smtpUsername, "Performance  Accelerator"),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(toEmail));
                

                await smtpClient.SendMailAsync(mail);
                model.IsSuccess = true;
                model.Messsage = "Mail send Successfully.";

            }
            catch (Exception ex)
            {
                 throw new Exception($"Failed to send email: {ex.Message}");
            }
            return model;
        }
    }
}


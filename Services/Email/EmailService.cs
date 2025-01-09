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
                using (var smtpClient = new SmtpClient(Constant.SMTP_HOST, Constant.SMTP_PORT))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(Constant.SMTP_USERNAME, Constant.SMTP_PASSWORD);

                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(Constant.SMTP_USERNAME);
                        mail.Subject = subject;
                        mail.Body = htmlContent;
                        mail.IsBodyHtml = true;
                        mail.To.Add(toEmail);

                        await smtpClient.SendMailAsync(mail);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary
                throw new InvalidOperationException("An error occurred while sending email.", ex);
            }
        }

    }
}


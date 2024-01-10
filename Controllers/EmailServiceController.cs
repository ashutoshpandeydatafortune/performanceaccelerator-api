using DF_EvolutionAPI.Services.Email;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    public class EmailServiceController : Controller
    {
       private readonly IEmailService _emailService;
        public EmailServiceController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SendEmail(string toEmail, string subject, string htmlContent)
        {
            var result = _emailService.SendEmail(toEmail, subject, htmlContent);
            return Ok(result);
        }
    }
}

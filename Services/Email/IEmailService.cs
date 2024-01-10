using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Email
{
    public interface IEmailService
    {
       public Task<ResponseModel> SendEmail(string toEmail, string subject, string htmlContent);
    }
}

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Login
{
    public interface ILoginService
    {
        public Task<LoginResponse> ExternalLogin(UserAuthModel uam, IConfiguration configuration);
    }
}

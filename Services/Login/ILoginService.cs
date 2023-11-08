using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;

namespace DF_EvolutionAPI.Services.Login
{
    public interface ILoginService
    {
        public Task<LoginResponse> ExternalLogin(UserAuthModel uam, IConfiguration configuration);
    }
}

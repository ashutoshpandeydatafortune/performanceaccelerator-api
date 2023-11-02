using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Login
{
    public interface ILoginService
    {
        public Task<ResponseModel> ExternalLogin(UserAuthModel uam)
        {
            throw new System.NotImplementedException();
        }
    }
}

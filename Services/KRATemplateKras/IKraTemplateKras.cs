using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.KRATemplateKras
{
    public interface IKraTemplateKras
    {
        public Task<ResponseModel> AssignTemplateKras(PATtemplateKrasList paTemplateKras);      
    }
}

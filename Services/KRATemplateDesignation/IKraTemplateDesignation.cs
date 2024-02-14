using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.KRATemplateDesignation
{
    public interface IKraTemplateDesignation
    {
        public Task<ResponseModel> AssignTemplateDesingation(PATtemplateDesignationList paTemplateDesignation);       
    }
}

using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using DF_EvolutionAPI.Models.Response;
using static DF_EvolutionAPI.Models.Response.PATemplate;

namespace DF_EvolutionAPI.Services.KRATemplate
{
    public interface IKRATemplateService
    {
        public Task<ResponseModel> CreateKraTemplate(PATemplate templateModel);
        public Task<ResponseModel> UpdateKraTemplate(PATemplate templateModel);
        public Task<PATemplate> GetKraTemplateByIdDetails(int templateId);
       public Task<PATemplate> GetKraTemplateById(int templateId);
        public Task<List<PATemplate>> GetAllTemplates();
        public Task<ResponseModel> DeleteKraTemplateById(int id);
        public Task<ResponseModel> AssignDesingations(PATtemplateDesignationList paTemplateDesignation);
        public Task<ResponseModel> AssignKRAs(PATtemplateKrasList paTemplateKras);
        public Task<List<object>> GetAssignedKRAsByDesignationId(int designationId);
        public Task<List<UserKraResult>> GetAssignedUserKrasByDesignationId(int designationId);

    }
}

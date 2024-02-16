using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.KRATemplate
{
    public interface IKRATemplateService
    {
        public Task<ResponseModel> CreateKraTemplate(PATemplate templateModel);
        public Task<ResponseModel> UpdateKraTemplate(PATemplate templateModel);
        public Task<PATemplate> GetKraTemplateByIdDetails(int templateId);
       public Task<PATemplate> GetKraTemplateById(int templateId);
        public Task<List<PATemplate>> GetAllTemplates();
        public Task<ResponseModel> DeleteKraTemplateById(int Id);

    }
}

using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.KRATemplate
{
    public interface IKRATemplateService
    {
        public Task<ResponseModel> CreateKraTemplate(PATemplates templateModel);
        public Task<ResponseModel> UpdateKraTemplate(PATemplates templateModel);
        public Task<PATemplates> GetKraTemplatesById(int Id);
        public Task<ResponseModel> DeleteKraTemplateById(int Id);
        public Task<List<PATemplates>> GetAllTemplates();

    }
}

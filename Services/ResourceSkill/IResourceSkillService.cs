using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IResourceSkillService
    {
        //public Task<ResponseModel> CreateResourceSkill(ResourceSkillRequestModel resourceSkillModel);
        public Task<ResponseModel> UpdateResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel);
       
        public Task<List<FetchResourceSkill>> GetAllResourceSkills();
        //public Task<List<FetchResourceSkill>> GetResourceSkillsById(int resourceId);
        public Task<List<FetchResourceCategorySkills>> GetResourceSkillsById(int resourceId);
        public Task<List<FetchResourceSkill>> GetResourcesBySkill(int skillId, int resourceId);


    }
}

using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Services
{
    public interface IResourceSkillService
    {
        public Task<ResponseModel> UpdateResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel);
        public Task<ResponseModel> InsertResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel);
        public Task<List<FetchResourceSkill>> GetAllResourceSkills();
        public Task<List<FetchResourceCategorySkills>> GetResourceSkillsById(int resourceId);
        public Task<List<FetchResourceSkill>> GetResourcesBySkill(SearchSkill searchSkillModel);
        public Task<ResponseModel> UpdateApprovalStatus(UpdateApprovalStatusRequestModel updateApproval);
        public Task<List<FetchResourceSkill>> CheckResourceSkillsUpdated(int resourceId);     
        public Task<ResponseModel> MarkResourceSkillAsInactive(ResourceSkillRequestModel resourceSkillRequestModel);
        public Task<List<FetchResourceSkill>> SearchTopResourcesBySkillOrSubSkill(SearchSkill searchSkillModel);
        public Task<List<FetchResourceSkills>> GetResourceSkills(int resourceId);


    }
}

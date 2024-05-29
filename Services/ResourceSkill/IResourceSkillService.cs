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
       
        public Task<List<FetchResourceSkill>> GetResourceSkills();
        public Task<List<FetchResourceSkill>> GetResourcesBySkill(string skillName);


    }
}

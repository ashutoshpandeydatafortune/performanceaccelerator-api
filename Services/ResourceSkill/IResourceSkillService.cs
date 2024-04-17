using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IResourceSkillService
    {
        public Task<ResponseModel> CreateResourceSkill(List<ResourceSkill> resourceSkillModel);
        public Task<ResponseModel> UpdateResourceSkill(List<ResourceSkill> resourceSkillModels);
        public Task<List<ResourceSkill>> GetResourceSkills(int? skillId, int? subSkillId);
    }
}

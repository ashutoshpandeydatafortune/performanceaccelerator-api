using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using DF_PA_API.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface ISkillService
    {
        public Task<List<Skill>> GetAllSkills();
        public Task<SkillDetails> GetSkillById(int id);
        public Task<ResponseModel> DeleteSkillById(int id);
        public Task<ResponseModel> CreateSkill(Skill skillModel);
        Task<List<ResourceSelectedSkill>> SearchBySkills(SearchSkill skillModel);
        public Task<ResponseModel> UpdateBySkillId(Skill skillModel);
    }
}

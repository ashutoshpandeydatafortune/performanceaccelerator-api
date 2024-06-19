using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface ISubSkillService
    {
        public Task<ResponseModel> CreateSubSkill(SubSkill subSkillModel);
        public Task<ResponseModel> UpdateBySubSkillId(SubSkill subSkillModel);
        public Task<ResponseModel> DeleteSubSkillById(int id);
        public Task<SubSkill> GetSubSkillById(int id);
        public Task<List<SubSkill>> GetAllSubSkills();
    }
}

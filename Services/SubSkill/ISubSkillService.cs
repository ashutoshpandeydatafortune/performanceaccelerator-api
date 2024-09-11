using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;

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

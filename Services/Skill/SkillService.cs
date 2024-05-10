using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Azure;
using DF_EvolutionAPI.ViewModels;
using DF_EvolutionAPI.Models;
namespace DF_EvolutionAPI.Services

{
    public class SkillService : ISkillService
    {
        private readonly DFEvolutionDBContext _dbContext;
        public SkillService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel> CreateSkill(Skill skillModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var skill = await _dbContext.Skills.FirstOrDefaultAsync(skill => skill.Name == skillModel.Name && skill.IsActive == 1);
                if (skill == null)
                {
                    skillModel.IsActive = 1;
                    skillModel.CreateDate = DateTime.UtcNow;

                    _dbContext.AddRange(skillModel);
                    model.Messsage = "Skill saved successfully.";

                    _dbContext.SaveChanges();
                    model.IsSuccess = true;
                }
                else
                {
                    model.Messsage = "Skill already exist.";
                    model.IsSuccess = false;
                }
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error :" + ex.Message;
            }
            return model;
        }

        public async Task<ResponseModel> UpdateBySkillId(Skill skillModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {

                var skill = await _dbContext.Skills.FirstOrDefaultAsync(skill => skill.Name == skillModel.Name && skill.IsActive == 1);
                if (skill != null)
                {
                    model.IsSuccess = false;
                    model.Messsage = "Skill with the same name already exists.";
                    return model;
                }
                else
                {
                    Skill updateSkill = _dbContext.Skills.Find(skillModel.SkillId);
                    if (updateSkill != null)
                    {
                        updateSkill.Name = skillModel.Name;
                        updateSkill.Description = skillModel.Description;
                        updateSkill.IsActive = 1;
                        updateSkill.UpdateBy = skillModel.UpdateBy;
                        updateSkill.UpdateDate = DateTime.Now;

                        _dbContext.Update(updateSkill);
                        _dbContext.SaveChanges();

                        model.Messsage = "Skill updated successfully.";
                        model.IsSuccess = true;
                    }

                    else
                    {
                        model.Messsage = "Skill already exist.";
                        model.IsSuccess = false;
                    }
                }


            }
            catch (Exception ex)
            {
                model.Messsage = "Error" + ex.Message;
                model.IsSuccess = false;
            }

            return model;
        }

        public async Task<List<Skill>> GetAllSkills()
        {
           return await _dbContext.Skills.Where(skill => skill.IsActive == 1).ToListAsync(); 
        }

       public async Task<SkillDetails> GetSkillById(int id)
        {
            var result = await (
                from skill in _dbContext.Skills
                join subskill in _dbContext.SubSkills.Where(s => s.IsActive == 1)
         on skill.SkillId equals subskill.SkillId into subSkillsGroup
                from subskill in subSkillsGroup.DefaultIfEmpty()
                where skill.SkillId == id && skill.IsActive == 1
                select new SkillDetails // Create an instance of Skill
                {
                    SkillId = skill.SkillId,
                    Name = skill.Name,
                    Description = skill.Description,
                    subSkills = _dbContext.SubSkills.Where(skill => skill.IsActive == 1 && skill.SkillId == id).Select(sub => new SubSkill // Create an instance of SubSkill for each related subskill
                    {
                        SkillId = sub.SkillId,
                        SubSkillId = sub.SubSkillId,
                        Name = sub.Name,
                        Description = sub.Description,                       
                    }).ToList()
                }
            ).FirstOrDefaultAsync();

            return result;
        }
      

            public async Task<ResponseModel> DeleteSkillById(int id)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteSkill = _dbContext.Skills.Find(id);
                if (deleteSkill != null)
                {
                    deleteSkill.IsActive = 0;
                    deleteSkill.UpdateDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                    model.Messsage = "Skill deleted successfully.";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Skill not found.";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error" + ex.Message;

            }
            return model;
        }
    }
}

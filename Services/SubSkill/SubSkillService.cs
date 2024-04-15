using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DF_EvolutionAPI.Services
{
    public class SubSkillService : ISubSkillService
    {
        private readonly DFEvolutionDBContext _dbContext;

        public SubSkillService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel> CreateSubSkill(SubSkill subSkillModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var subSkill = await _dbContext.SubSkills.FirstOrDefaultAsync(subSkill => subSkill.Name == subSkillModel.Name && subSkill.IsActive == 1);
                if (subSkill == null)
                {
                    subSkillModel.IsActive = 1;
                    subSkillModel.CreateDate = DateTime.UtcNow;

                    _dbContext.Add(subSkillModel);
                    model.Messsage = "SubSkill saved successfully.";

                    _dbContext.SaveChanges();
                    model.IsSuccess = true;
                }
                else
                {
                    model.Messsage = "SubSkill already exist.";
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

        public async Task<ResponseModel> UpdateBySubSkillId(SubSkill subSkillModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {

                var skill = await _dbContext.SubSkills.FirstOrDefaultAsync(skill => skill.Name == subSkillModel.Name && skill.IsActive == 1);
                if (skill != null)
                {
                    model.IsSuccess = false;
                    model.Messsage = "SubSkill with the same name already exists.";
                    return model;
                }
                else
                {
                    SubSkill updateSubSkill = _dbContext.SubSkills.Find(subSkillModel.SubSkillId);
                    if (updateSubSkill != null)
                    {
                        updateSubSkill.Name = subSkillModel.Name;
                        updateSubSkill.SkillId = subSkillModel.SkillId;
                        updateSubSkill.Description = subSkillModel.Description;
                        updateSubSkill.IsActive = 1;
                        updateSubSkill.UpdateBy = subSkillModel.UpdateBy;
                        updateSubSkill.UpdateDate = DateTime.Now;

                        _dbContext.Update(updateSubSkill);
                        _dbContext.SaveChanges();

                        model.Messsage = "SubSkill updated successfully.";
                        model.IsSuccess = true;
                    }

                    else
                    {
                        model.Messsage = "SubSkill already exist.";
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


        public async Task<List<SubSkill>> GetAllSubSkills()
        {
            return await _dbContext.SubSkills.Where(subskill => subskill.IsActive == 1).ToListAsync();
        }

        public async Task<SubSkill> GetSubSkillById(int id)
        {
            return await _dbContext.SubSkills.Where(subskill => subskill.SkillId == id && subskill.IsActive == 1).FirstOrDefaultAsync() ?? throw new Exception("SubSkill does not exist."); ;
        }

        public async Task<ResponseModel> DeleteSubSkillById(int id)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteSubSkill = _dbContext.SubSkills.Find(id);
                if (deleteSubSkill != null)
                {
                    deleteSubSkill.IsActive = 0;
                    deleteSubSkill.UpdateDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                    model.Messsage = "SubSkill deleted successfully.";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "SubSkill not found.";
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

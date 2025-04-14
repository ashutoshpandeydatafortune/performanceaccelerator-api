using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Utils;

namespace DF_EvolutionAPI.Services
{
    public class SubSkillService : ISubSkillService
    {
        private readonly DFEvolutionDBContext _dbContext;
        private readonly ILogger<SubSkillService> _logger;

        public SubSkillService(DFEvolutionDBContext dbContext, ILogger<SubSkillService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseModel> CreateSubSkill(SubSkill subSkillModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var subSkill = await (
                     from skill in _dbContext.Skills
                     join subskill in _dbContext.SubSkills on skill.SkillId equals subskill.SkillId
                     where skill.SkillId == subSkillModel.SkillId && subskill.Name == subSkillModel.Name && skill.IsActive == (int)Status.IS_ACTIVE && subskill.IsActive == (int)Status.IS_ACTIVE
                     select subskill
                 ).FirstOrDefaultAsync();

                if (subSkill == null)
                {
                    subSkillModel.IsActive = (int)Status.IS_ACTIVE;
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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }
            return model;
        }

        public async Task<ResponseModel> UpdateBySubSkillId(SubSkill subSkillModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var subSkill = await (
                     from skill in _dbContext.Skills
                     join subskill in _dbContext.SubSkills on skill.SkillId equals subskill.SkillId
                     where skill.SkillId == subSkillModel.SkillId && subskill.Name == subSkillModel.Name && skill.Description == subSkillModel.Description && skill.IsActive == (int)Status.IS_ACTIVE && subskill.IsActive == (int)Status.IS_ACTIVE
                     select subskill
                 ).FirstOrDefaultAsync();

                if (subSkill != null)
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
                        updateSubSkill.IsActive = (int)Status.IS_ACTIVE;
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
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                
            }
            return model;
        }


        public async Task<List<SubSkill>> GetAllSubSkills()
        {
            return await _dbContext.SubSkills.Where(subskill => subskill.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<SubSkill> GetSubSkillById(int id)
        {
            return await _dbContext.SubSkills.Where(subskill => subskill.SkillId == id && subskill.IsActive == (int)Status.IS_ACTIVE).FirstOrDefaultAsync() ?? throw new Exception("SubSkill does not exist."); ;
        }

        public async Task<ResponseModel> DeleteSubSkillById(int id)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteSubSkill = _dbContext.SubSkills.Find(id);
                if (deleteSubSkill != null)
                {
                    deleteSubSkill.IsActive = (int)Status.IN_ACTIVE;
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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

            }
            return model;
        }
    }
}

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static DF_EvolutionAPI.Models.ResourceSkill;

namespace DF_EvolutionAPI.Services
{
    public class ResourceSkillService : IResourceSkillService

    {
        private readonly DFEvolutionDBContext _dbContext;

        public ResourceSkillService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel> CreateResourceSkill(List<ResourceSkill> resourceSkillModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                foreach (var item in resourceSkillModel)
                {

                    var resourceSkill = _dbContext.ResourceSkills.Where(rs =>
                     rs.SkillId == item.SkillId && rs.SubSkillId == item.SubSkillId && rs.ResourceId == item.ResourceId).ToList();

                    if (resourceSkill.Count == 0)
                    {
                        item.IsActive = 1;
                        item.CreateDate = DateTime.Now;

                        await _dbContext.AddAsync(item);
                    }
                    else
                    {
                        model.IsSuccess = false;
                        model.Messsage = "Resource Skill already exist.";
                    }
                }

                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;
                model.Messsage = "Resource Skill saved successfully.";
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error: " + ex.Message;
            }

            return model;
        }

        public async Task<ResponseModel> UpdateResourceSkill(List<ResourceSkill> resourceSkillModels)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                foreach (var resourceSkillModel in resourceSkillModels)
                {
                    // Find the existing resource skill in the database
                    var existingResourceSkill = await _dbContext.ResourceSkills.FindAsync(resourceSkillModel.ResourceSkillId);
                    if (existingResourceSkill != null)
                    {
                        // Update the properties of the existing resource skill
                        existingResourceSkill.SkillId = resourceSkillModel.SkillId;
                        existingResourceSkill.SubSkillId = resourceSkillModel.SubSkillId;
                        existingResourceSkill.ResourceId = resourceSkillModel.ResourceId;
                        existingResourceSkill.Experience = resourceSkillModel.Experience;
                        existingResourceSkill.IsActive = 1;
                        existingResourceSkill.UpdateBy = resourceSkillModel.UpdateBy;
                        existingResourceSkill.UpdateDate = DateTime.Now; // Update the update date
                    }
                    else
                    {
                        model.IsSuccess = false;
                        model.Messsage = "Resource skill does not exist.";
                        return model; // Exit the method if resource skill not found
                    }
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                model.IsSuccess = true;
                model.Messsage = "Resource skills updated successfully.";
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error: " + ex.Message;
            }

            return model;
        }

        public async Task<List<ResourceSkill>> GetResourceSkills(int? skillId, int? subSkillId)
        {

            if ((skillId != null && skillId != 0) && (subSkillId == null || subSkillId == 0))
            {
                // Filter by skillId only
                return await _dbContext.ResourceSkills
                    .Where(rs => rs.SkillId == skillId && rs.IsActive == 1)
                    .ToListAsync();
            }
            else if ((skillId == null || skillId == 0) && (subSkillId != null && subSkillId != 0))
            {
                // Filter by subSkillID only
                return await _dbContext.ResourceSkills
                    .Where(rs => rs.SubSkillId == subSkillId && rs.IsActive == 1)
                    .ToListAsync();
            }
            else if ((skillId != null && skillId != 0) && (subSkillId != null && subSkillId != 0))
            {
                // Filter by both skillId and subSkillID
                return await _dbContext.ResourceSkills
                    .Where(rs => rs.SkillId == skillId && rs.SubSkillId == subSkillId && rs.IsActive == 1)
                    .ToListAsync();
            }
            else
            {
                // Handle other cases or return all ResourceSkills
                return await _dbContext.ResourceSkills
                    .Where(rs => rs.IsActive == 1)
                    .ToListAsync();
            }
        }
    }
}


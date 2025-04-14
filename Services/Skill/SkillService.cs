using System;
using System.Linq;
using DF_PA_API.Models;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using DF_EvolutionAPI.Utils;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Services

{
    public class SkillService : ISkillService
    {
        private readonly DFEvolutionDBContext _dbContext;
        private readonly ILogger<SkillService> _logger;
        public SkillService(DFEvolutionDBContext dbContext, ILogger<SkillService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseModel> CreateSkill(Skill skillModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var skill = await (from s in _dbContext.Skills
                                   join c in _dbContext.Categories
                                   on s.CategoryId equals c.CategoryId
                                   where s.Name == skillModel.Name && s.IsActive == (int)Status.IS_ACTIVE && c.IsActive == (int)Status.IS_ACTIVE
                                   select s).FirstOrDefaultAsync();
                if (skill == null)
                {
                    skillModel.IsActive = (int)Status.IS_ACTIVE;
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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }
            return model;
        }

        public async Task<ResponseModel> UpdateBySkillId(Skill skillModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var skill = await _dbContext.Skills.FirstOrDefaultAsync(skill => skill.Name == skillModel.Name
                 && skill.CategoryId == skillModel.CategoryId && skill.SkillId != skillModel.SkillId && skill.IsActive == (int)Status.IS_ACTIVE);
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
                        updateSkill.IsActive = (int)Status.IS_ACTIVE;
                        updateSkill.UpdateBy = skillModel.UpdateBy;
                        updateSkill.UpdateDate = DateTime.Now;
                        updateSkill.CategoryId = skillModel.CategoryId;

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
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

            }
            return model;
        }

        public async Task<List<Skill>> GetAllSkills()
        {
            var skills = await (from skill in _dbContext.Skills
                                join category in _dbContext.Categories
                                on skill.CategoryId equals category.CategoryId
                                where skill.IsActive == (int)Status.IS_ACTIVE && category.IsActive == (int)Status.IS_ACTIVE
                                orderby skill.SkillId
                                select new Skill
                                {
                                    SkillId = skill.SkillId,
                                    Name = skill.Name,
                                    Description = skill.Description,
                                    IsActive = skill.IsActive,
                                    CreateBy = skill.CreateBy,
                                    UpdateBy = skill.UpdateBy,
                                    CreateDate = skill.CreateDate,
                                    UpdateDate = skill.UpdateDate,
                                    CategoryId = skill.CategoryId,
                                    CategoryName = category.CategoryName,
                                    SubSkills = skill.SubSkills.Where(sub => sub.IsActive == (int)Status.IS_ACTIVE).ToList()
                                }).ToListAsync();

            return skills;
        }

        public async Task<SkillDetails> GetSkillById(int id)
        {
            var result = await (
                from skill in _dbContext.Skills
                join category in _dbContext.Categories on skill.CategoryId equals category.CategoryId
                join subskill in _dbContext.SubSkills.Where(s => s.IsActive == (int)Status.IS_ACTIVE)
                on skill.SkillId equals subskill.SkillId into subSkillsGroup
                from subskill in subSkillsGroup.DefaultIfEmpty()
                where skill.SkillId == id && skill.IsActive == (int)Status.IS_ACTIVE
                select new SkillDetails // Create an instance of Skill
                {
                    SkillId = skill.SkillId,
                    Name = skill.Name,
                    Description = skill.Description,
                    CategoryId = skill.CategoryId,
                    CategoryName = category.CategoryName,
                    subSkills = _dbContext.SubSkills.Where(skill => skill.IsActive == (int)Status.IS_ACTIVE && skill.SkillId == id).Select(sub => new SubSkill // Create an instance of SubSkill for each related subskill
                    {
                        SkillId = sub.SkillId,
                        SubSkillId = sub.SubSkillId,
                        Name = sub.Name,
                        Description = sub.Description,
                    }).ToList()
                }).FirstOrDefaultAsync();

                return result;
        }


        //display skill coording to category
        public async Task<List<CategoryDetails>> GetSkillByCategoryId(int id)
        {
            var categoryWiseSkills = await (
             from category in _dbContext.Categories
             join skill in _dbContext.Skills on category.CategoryId equals skill.CategoryId
             where skill.IsActive == (int)Status.IS_ACTIVE && category.CategoryId == id
             orderby category.CategoryName, skill.SkillId
             group skill by new
             {
                 category.CategoryId,
                 category.CategoryName
             } into categoryGroup
             select new CategoryDetails
             {
                 CategoryId = categoryGroup.Key.CategoryId,
                 CategoryName = categoryGroup.Key.CategoryName,
                 Skills = categoryGroup.Select(skill => new Skill
                 {
                     SkillId = skill.SkillId,
                     Name = skill.Name,
                     Description = skill.Description,
                     IsActive = skill.IsActive,
                     CreateBy = skill.CreateBy,
                     UpdateBy = skill.UpdateBy,
                     CreateDate = skill.CreateDate,
                     UpdateDate = skill.UpdateDate,
                     CategoryId = skill.CategoryId,
                     CategoryName = categoryGroup.Key.CategoryName,
                     SubSkills = skill.SubSkills.Where(sub => sub.IsActive == (int)Status.IS_ACTIVE).ToList()
                 }).ToList()
             }).ToListAsync();

            return categoryWiseSkills;

        }



        public async Task<ResponseModel> DeleteSkillById(int id)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteSkill = _dbContext.Skills.Find(id);
                if (deleteSkill != null)
                {
                    deleteSkill.IsActive = (int)Status.IN_ACTIVE;
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


        public async Task<List<Resource>> SearchBySkills(SearchSkill skillModel)
        {
            var query = _dbContext.Resources.AsQueryable();

            if (!string.IsNullOrEmpty(skillModel.SearchKey))
            {
                // Join with ResourceSkills, Skills, and SubSkills to filter by SearchKey
                query = from r in _dbContext.Resources
                        join rs in _dbContext.ResourceSkills on r.ResourceId equals rs.ResourceId
                        join skill in _dbContext.Skills on rs.SkillId equals skill.SkillId
                        join subskill in _dbContext.SubSkills on rs.SubSkillId equals subskill.SubSkillId
                        where skill.Name.Contains(skillModel.SearchKey) || subskill.Name.Contains(skillModel.SearchKey)
                        group r by r.ResourceId into groupedResources
                        select groupedResources.FirstOrDefault();

            }
            else
            {
                if (skillModel.SkillIds != null && skillModel.SkillIds.Count > 0)
                {
                    query = query.Where(r => r.ResourceSkills.Any(rs => skillModel.SkillIds.Contains(rs.SkillId.Value)));
                }

                if (skillModel.SubSkillIds != null && skillModel.SubSkillIds.Count > 0)
                {
                    query = query.Where(resource =>
                    resource.ResourceSkills.Any(resourceSkill =>
                    skillModel.SubSkillIds.Contains(resourceSkill.SubSkillId.Value))); // Check if the SubSkillId matches

                }
            }

            return await query.ToListAsync();
        }
    }
}

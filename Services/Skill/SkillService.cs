﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Azure;
using DF_EvolutionAPI.ViewModels;
using DF_EvolutionAPI.Models;
using DF_PA_API.Models.Response;
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

                var skill = await _dbContext.Skills.FirstOrDefaultAsync(skill => skill.Name == skillModel.Name && skill.Description == skillModel.Description && skill.IsActive == 1);
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


        //        public async Task<List<Resource>> SearchBySkills(SearchSkill skillModel)
        //        {
        //            var query = _dbContext.Resources.AsQueryable();

        //            if (!string.IsNullOrEmpty(skillModel.SearchKey))
        //            {
        //                // Join with ResourceSkills, Skills, and SubSkills to filter by SearchKey
        //                query = from r in _dbContext.Resources
        //                        join rs in _dbContext.ResourceSkills on r.ResourceId equals rs.ResourceId
        //                        join skill in _dbContext.Skills on rs.SkillId equals skill.SkillId
        //                        join subskill in _dbContext.SubSkills on rs.SubSkillId equals subskill.SubSkillId
        //                        where skill.Name.Contains(skillModel.SearchKey) || subskill.Name.Contains(skillModel.SearchKey)
        //                        group r by r.ResourceId into groupedResources
        //                        select groupedResources.FirstOrDefault();

        //            }
        //            else
        //            {
        //                if (skillModel.SkillIds != null && skillModel.SkillIds.Count > 0)
        //                {
        //                    query = query.Where(r => r.ResourceSkills.Any(rs => skillModel.SkillIds.Contains(rs.SkillId.Value)));
        //                }

        //                if (skillModel.SubSkillIds != null && skillModel.SubSkillIds.Count > 0)
        //                {
        //                    query = query.Where(resource =>
        //                    resource.ResourceSkills.Any(resourceSkill =>
        //                      skillModel.SubSkillIds.Contains(resourceSkill.SubSkillId.Value) // Check if the SubSkillId matches
        //    )
        //);
        //                }
        //            }

        //            return await query.ToListAsync();
        //        }


        //public async Task<List<ResourceSelectedSkill>> SearchBySkills(SearchSkill skillModel)
        //{
        //    var query = _dbContext.ResourceSkills.AsQueryable();

        //    if (!string.IsNullOrEmpty(skillModel.SearchKey))
        //    {
        //        var results =
        //    from rs in _dbContext.ResourceSkills
        //    join r in _dbContext.Resources on rs.ResourceId equals r.ResourceId
        //    join s in _dbContext.Skills on rs.SkillId equals s.SkillId
        //    where s.Name.Contains(skillModel.SearchKey)
        //    group new { r, rs, s } by r.ResourceId into grouped
        //    select new ResourceSelectedSkill
        //    {
        //        ResourceId = grouped.Key,
        //        ResourceName = grouped.First().r.ResourceName,
        //        ResourceExperience = grouped.First().r.TotalYears.HasValue ? grouped.First().r.TotalYears.Value : 0,
        //        DateOfJoining =grouped.First().r.DateOfJoin,
        //        SkillId = grouped.First().s.SkillId
        //    };

        //        // Execute the query and return the results
        //        return await results.ToListAsync();
        //    }
        //    else
        //    {

        //    // If no search key is provided, return an empty list or handle as needed
        //    return new List<ResourceSelectedSkill>();
        //    }
        //}


        public async Task<List<ResourceSelectedSkill>> SearchBySkills(SearchSkill skillModel)
        {
            if (!string.IsNullOrEmpty(skillModel.SearchKey))
            {
                // Filter by search key
                var results =
                    from rs in _dbContext.ResourceSkills
                    join r in _dbContext.Resources on rs.ResourceId equals r.ResourceId
                    join s in _dbContext.Skills on rs.SkillId equals s.SkillId
                    where s.Name.Contains(skillModel.SearchKey)
                    group new { r, rs, s } by r.ResourceId into grouped
                    select new ResourceSelectedSkill
                    {
                        ResourceId = grouped.Key,
                        ResourceName = grouped.First().r.ResourceName,
                        ResourceExperience = grouped.First().r.TotalYears.HasValue ? grouped.First().r.TotalYears.Value : 0,
                        DateOfJoining = grouped.First().r.DateOfJoin,
                        SkillId = grouped.First().s.SkillId
                    };

                // Execute the query and return the results
                return await results.ToListAsync();
            }
            else
            {
                // Initialize the base query
                var query = _dbContext.Resources.AsQueryable();

                // Filter by SkillIds if provided
                if (skillModel.SkillIds != null && skillModel.SkillIds.Count > 0)
                {
                    query = query.Where(r => r.ResourceSkills.Any(rs => skillModel.SkillIds.Contains(rs.SkillId.Value)));
                }

                // Filter by SubSkillIds if provided
                if (skillModel.SubSkillIds != null && skillModel.SubSkillIds.Count > 0)
                {
                    query = query.Where(r => r.ResourceSkills.Any(rs => skillModel.SubSkillIds.Contains(rs.SubSkillId.Value)));
                }

                // Project the results into ResourceSelectedSkill
                var results = query.Select(r => new ResourceSelectedSkill
                {
                    ResourceId = r.ResourceId,
                    ResourceName = r.ResourceName,
                    ResourceExperience = r.TotalYears.HasValue ? r.TotalYears.Value : 0,
                    DateOfJoining = r.DateOfJoin ?? DateTime.MinValue,
                    SkillId = r.ResourceSkills.FirstOrDefault().SkillId ?? 0 // Get first SkillId
                });

                // Execute the query and return the results
                return await results.ToListAsync();
            }
        }



    }
}

using System;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;

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
      
        public async Task<ResponseModel> UpdateResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                // Retrieve the resource ID from the request model
                int resourceId = resourceSkillRequestModel.ResourceId;

                // Delete existing resource skills
                var existingResourceSkills = _dbContext.ResourceSkills
                    .Where(rs => rs.ResourceId == resourceId && rs.IsActive == 1)
                    .ToList();

                if (existingResourceSkills.Any())
                {
                    _dbContext.ResourceSkills.RemoveRange(existingResourceSkills);
                }

                // Iterate through skill categories
                foreach (var category in resourceSkillRequestModel.SkillCategories)
                {
                    foreach (var skill in category.Skills)
                    {
                        if (skill.SubSkills == null || !skill.SubSkills.Any())
                        {
                            // No sub-skills, add the skill with default/null sub-skill values
                            var newResourceSkill = new ResourceSkill
                            {
                                SkillId = skill.SkillId,
                                SubSkillId = null, // Assuming SubSkillId can be nullable, set to default/null
                                ResourceId = resourceId,
                                SkillExperience = skill.SkillExperience,
                                SubSkillExperience = null,
                                IsActive = resourceSkillRequestModel.IsActive,
                                CreateBy = resourceSkillRequestModel.CreateBy,
                                CreateDate = DateTime.Now
                            };
                            _dbContext.ResourceSkills.Add(newResourceSkill);
                        }
                        else
                        {
                            foreach (var subSkill in skill.SubSkills)
                            {
                                var newResourceSkill = new ResourceSkill
                                {
                                    SkillId = skill.SkillId,
                                    SubSkillId = subSkill.SubSkillId,
                                    ResourceId = resourceId,
                                    SkillExperience = skill.SkillExperience,
                                    SubSkillExperience = subSkill.SubSkillExperience,
                                    IsActive = resourceSkillRequestModel.IsActive,
                                    CreateBy = resourceSkillRequestModel.CreateBy,
                                    CreateDate = DateTime.Now
                                };
                                _dbContext.ResourceSkills.Add(newResourceSkill);
                            }
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();

                model.IsSuccess = true;
                model.Messsage = "Resource skill and subskills saved successfully.";
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error: " + ex.Message;
            }
            return model;
        }

        public async Task<List<FetchResourceSkill>> GetAllResourceSkills()
        {
            var result = await (
                from rs in _dbContext.ResourceSkills
                join r in _dbContext.Resources on rs.ResourceId equals r.ResourceId
                join s in _dbContext.Skills on rs.SkillId equals s.SkillId into skillGroup
                from skill in skillGroup.DefaultIfEmpty()
                join sub in _dbContext.SubSkills on rs.SubSkillId equals sub.SubSkillId into subSkillGroup
                from subSkill in subSkillGroup.DefaultIfEmpty()
                where rs.IsActive == 1 
                select new
                {
                    r.ResourceId,
                    r.ResourceName,
                    NewSkillId = skill.SkillId,
                    SkillName = skill.Name,
                    NewSubSkillId = subSkill.SubSkillId,
                    SubSkillName = subSkill.Name
                }
            ).ToListAsync();

            // Group the results by ResourceId
            var groupedResults = result.GroupBy(r => r.ResourceId);

            // Create a list to hold the final FetchResourceSkill objects
            var finalResult = new List<FetchResourceSkill>();

            // Iterate over each group and create the FetchResourceSkill objects
            foreach (var group in groupedResults)
            {
                var skills = new List<SkillModel>();

                // Group skills and subskills
                var skillGroups = group.GroupBy(r => r.NewSkillId);

                foreach (var skillGroup in skillGroups)
                {
                    var subSkills = skillGroup
                        .Where(r => r.NewSubSkillId != 0)
                        .Select(r => new SubSkillModel
                        {
                            SubSkillId = r.NewSubSkillId,
                            SubSkillName = r.SubSkillName
                        }).ToList();

                    var skillModel = new SkillModel
                    {
                        SkillId = skillGroup.Key,
                        SkillName = skillGroup.First().SkillName,
                        SubSkills = subSkills
                    };

                    skills.Add(skillModel);
                }

                var fetchResourceSkill = new FetchResourceSkill
                {
                    ResourceId = group.Key,
                    ResourceName = group.First().ResourceName,
                    Skills = skills
                };

                finalResult.Add(fetchResourceSkill);
            }

            return finalResult;
        }

        public async Task<List<FetchResourceCategorySkills>> GetResourceSkillsById(int resourceId)
        {
            var result = await (
                from rs in _dbContext.ResourceSkills
                join r in _dbContext.Resources on rs.ResourceId equals r.ResourceId
                join s in _dbContext.Skills on rs.SkillId equals s.SkillId into skillGroup
                from skill in skillGroup.DefaultIfEmpty()
                join sub in _dbContext.SubSkills on rs.SubSkillId equals sub.SubSkillId into subSkillGroup
                from subSkill in subSkillGroup.DefaultIfEmpty()
                join c in _dbContext.Categories on skill.CategoryId equals c.CategoryId into categoryGroup
                from category in categoryGroup.DefaultIfEmpty()
                where rs.IsActive == 1 && r.ResourceId == resourceId
                select new
                {
                    r.ResourceId,
                    r.ResourceName,
                    rs.SkillExperience,
                    rs.SubSkillExperience,
                    NewSkillId = (int?)skill.SkillId,
                    SkillName = skill.Name,
                    CategoryName = category.CategoryName,
                    NewSubSkillId = (int?)subSkill.SubSkillId,
                    SubSkillName = subSkill.Name,
                }
            ).ToListAsync();

            // Group the results by ResourceId
            var groupedResults = result.GroupBy(r => r.ResourceId);

            // Create a list to hold the final FetchResourceSkill objects
            var finalResult = new List<FetchResourceCategorySkills>();

            // Iterate over each group and create the FetchResourceSkill objects
            foreach (var group in groupedResults)
            {
                var categoryWiseSkills = group
                    .GroupBy(r => r.CategoryName)
                    .Select(categoryGroup => new CategorySkillModel
                    {
                        CategoryName = categoryGroup.Key,
                        Skills = categoryGroup
                            .GroupBy(r => r.NewSkillId)
                            .Select(skillGroup => new SkillModel
                            {
                                SkillId = skillGroup.Key.HasValue ? skillGroup.Key.Value : 0,
                                SkillName = skillGroup.First().SkillName,
                                SkillExperience = skillGroup.First().SkillExperience,
                                SubSkills = skillGroup
                                    .Where(r => r.NewSubSkillId != null)
                                    .Select(r => new SubSkillModel
                                    {
                                        SkillId = r.NewSkillId,
                                        SubSkillId = r.NewSubSkillId,
                                        SubSkillName = r.SubSkillName,
                                        SubSkillExperience = r.SubSkillExperience
                                    }).ToList()
                            }).ToList()
                    }).ToList();

                var fetchResourceSkill = new FetchResourceCategorySkills
                {
                    ResourceId = group.Key,
                    ResourceName = group.First().ResourceName,
                    CategoryWiseSkills = categoryWiseSkills
                };

                finalResult.Add(fetchResourceSkill);
            }

            return finalResult;
        }



        public async Task<List<FetchResourceSkill>> GetResourcesBySkill(int skillId, int resourceId)
        {
            //var skillId = _dbContext.Skills.Where(name => name.Name == skillName)
            //       .Select(name => name.SkillId)
            //       .FirstOrDefault();
            var result = await (
                from rs in _dbContext.ResourceSkills
                join r in _dbContext.Resources on rs.ResourceId equals r.ResourceId
                join s in _dbContext.Skills on rs.SkillId equals s.SkillId into skillGroup
                from skill in skillGroup.DefaultIfEmpty()
                join sub in _dbContext.SubSkills on rs.SubSkillId equals sub.SubSkillId into subSkillGroup
                from subSkill in subSkillGroup.DefaultIfEmpty()
                where rs.IsActive == 1 && rs.SkillId == skillId && r.ResourceId == resourceId // Filter by SkillId
                select new
                {
                    r.ResourceId,
                    r.ResourceName,
                    NewSkillId = skill.SkillId,
                    SkillName = skill.Name,
                    NewSubSkillId = subSkill.SubSkillId,
                    SubSkillName = subSkill.Name
                }
            ).ToListAsync();

            // Group the results by ResourceId
            var groupedResults = result.GroupBy(r => r.ResourceId);

            // Create a list to hold the final FetchResourceSkill objects
            var finalResult = new List<FetchResourceSkill>();

            // Iterate over each group and create the FetchResourceSkill objects
            foreach (var group in groupedResults)
            {
                var skills = new List<SkillModel>();

                // Group skills and subskills
                var skillGroups = group.GroupBy(r => r.NewSkillId);

                foreach (var skillGroup in skillGroups)
                {
                    var subSkills = skillGroup
                        .Where(r => r.NewSubSkillId != 0)
                        .Select(r => new SubSkillModel
                        {
                            SubSkillId = r.NewSubSkillId,
                            SubSkillName = r.SubSkillName
                        }).ToList();

                    var skillModel = new SkillModel
                    {
                        SkillId = skillGroup.Key,
                        SkillName = skillGroup.First().SkillName,
                        SubSkills = subSkills
                    };

                    skills.Add(skillModel);
                }

                var fetchResourceSkill = new FetchResourceSkill
                {
                    //ResourceId = group.Key,
                    //ResourceName = group.First().ResourceName,
                    Skills = skills
                };

                finalResult.Add(fetchResourceSkill);
            }
            return finalResult;
        }
    }
}


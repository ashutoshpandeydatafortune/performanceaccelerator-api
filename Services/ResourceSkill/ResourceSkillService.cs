using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;


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
                    .Where(rs => rs.ResourceId == resourceId && rs.IsActive == (int)Status.IS_ACTIVE)
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
                                SkillVersion = skill.SkillVersion,
                                SkillDescription = skill.SkillDescription,
                                SubSkillExperience = null,
                                IsActive = (int)Status.IS_ACTIVE,
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
                                    SkillVersion = skill.SkillVersion,
                                    SkillDescription = skill.SkillDescription,
                                    SubSkillExperience = subSkill.SubSkillExperience,
                                    SubSkillVersion = subSkill.SubSkillVersion,
                                    SubSkillDescription = subSkill.SubSkillDescription,
                                    IsActive = (int)Status.IS_ACTIVE,
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
                where rs.IsActive == (int)Status.IS_ACTIVE
                select new
                {
                    r.ResourceId,
                    r.ResourceName,
                    rs.SkillExperience,
                    rs.SkillVersion,
                    rs.SkillDescription,
                    rs.SubSkillExperience,
                    rs.SubSkillVersion,
                    rs.SubSkillDescription,
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
                            SubSkillName = r.SubSkillName,
                            SubSkillExperience = r.SkillExperience,
                            SubSkillVersion = r.SubSkillVersion,
                            SubSkillDescription = r.SubSkillDescription,
                            
                            
                        }).ToList();

                    var skillModel = new SkillModel
                    {
                        SkillId = skillGroup.Key,
                        SkillName = skillGroup.First().SkillName,
                        SkillExperience = skillGroup.First().SkillExperience,
                        SkillVersion = skillGroup.First().SkillVersion,
                        SkillDescription = skillGroup.First().SkillDescription,
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
                where rs.IsActive == (int)Status.IS_ACTIVE && r.ResourceId == resourceId
                select new
                {
                    r.ResourceId,
                    r.ResourceName,
                    rs.SkillExperience,
                    rs.SkillVersion,
                    rs.SkillDescription,
                    rs.SubSkillExperience,
                    rs.SubSkillVersion,
                    rs.SubSkillDescription,
                    category.CategoryId,
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
                     .GroupBy(r => r.CategoryId)
                    .Select(categoryGroup => new CategorySkillModel
                    {
                        CategoryName = categoryGroup.First().CategoryName, // Take CategoryName from the first item in the group
                        CategoryId = categoryGroup.Key,
                        Skills = categoryGroup
                            .GroupBy(r => r.NewSkillId)
                            .Select(skillGroup => new SkillModel
                            {
                                SkillId = skillGroup.Key.HasValue ? skillGroup.Key.Value : 0,
                                SkillName = skillGroup.First().SkillName,
                                SkillExperience = skillGroup.First().SkillExperience,
                                SkillVersion = skillGroup.First().SkillVersion,
                                SkillDescription = skillGroup.First().SkillDescription,
                                
                                SubSkills = skillGroup
                                    .Where(r => r.NewSubSkillId != null)
                                    .Select(r => new SubSkillModel
                                    {
                                        SkillId = r.NewSkillId,
                                        SubSkillId = r.NewSubSkillId,
                                        SubSkillName = r.SubSkillName,
                                        SubSkillExperience = r.SubSkillExperience,
                                        SubSkillVersion = r.SubSkillVersion,
                                        SubSkillDescription = r.SubSkillDescription,
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
        public async Task<List<FetchResourceSkill>> GetResourcesBySkill(SearchSkill skillModel)
        {
            // Base query for resource, skills, and subskills
            var query = from rs in _dbContext.ResourceSkills
                        join r in _dbContext.Resources on rs.ResourceId equals r.ResourceId
                        join s in _dbContext.Skills on rs.SkillId equals s.SkillId into skillGroup
                        from skill in skillGroup.DefaultIfEmpty()
                        join sub in _dbContext.SubSkills on rs.SubSkillId equals sub.SubSkillId into subSkillGroup
                        from subSkill in subSkillGroup.DefaultIfEmpty()
                        select new
                        {
                            r.ResourceId,
                            r.ResourceName,
                            rs.SkillExperience,
                            rs.SkillVersion,
                            rs.SkillDescription,
                            rs.SubSkillExperience,
                            rs.SubSkillVersion,
                            rs.SubSkillDescription,
                            r.DateOfJoin,
                            r.TotalYears,
                            NewSkillId = skill.SkillId,
                            SkillName = skill.Name,
                            NewSubSkillId = subSkill != null ? subSkill.SubSkillId : (int?)null, // Allow null SubSkillId
                            SubSkillName = subSkill != null ? subSkill.Name : null // Allow null SubSkillName

                        };

            // Step 1: Identify matching resources based on SearchKey, SkillIds, or SubSkillIds
            IQueryable<int> matchedResourceIds = query.Select(q => q.ResourceId);

            // If SearchKey is provided, find resources that match the SearchKey
            if (!string.IsNullOrEmpty(skillModel.SearchKey))
            {
                matchedResourceIds = query
                    .Where(r => r.SkillName.Contains(skillModel.SearchKey) ||
                                r.SubSkillName.Contains(skillModel.SearchKey))
                    .Select(r => r.ResourceId);
            }
            else
            {
                // If SkillIds are provided, find resources that have matching SkillIds or SubSkillIds
                if (skillModel.SkillIds != null && skillModel.SkillIds.Count > 0)
                {
                    matchedResourceIds = query
                        .Where(r => skillModel.SkillIds.Contains(r.NewSkillId))
                        .Select(r => r.ResourceId);
                }

                // If SubSkillIds are provided, find resources that have matching SubSkillIds
                if (skillModel.SubSkillIds != null && skillModel.SubSkillIds.Count > 0)
                {
                    matchedResourceIds = query
                        .Where(r => skillModel.SubSkillIds.Contains((int)r.NewSubSkillId))
                        .Select(r => r.ResourceId);
                }
            }
            // Step 2: Fetch all skills for matched resources
            var filteredQuery = query.Where(r => matchedResourceIds.Contains(r.ResourceId));

            // Execute the filtered query
            var result = await filteredQuery.ToListAsync();

            // Group the results by ResourceId
            var groupedResults = result.GroupBy(r => r.ResourceId);

            // Create a list to hold the final FetchResourceSkill objects
            var finalResult = new List<FetchResourceSkill>();

            // Step 3: Iterate over each group and create the FetchResourceSkill objects
            foreach (var group in groupedResults)
            {
                var skills = new List<SkillModel>();

                // Group skills and subskills
                var skillGroups = group.GroupBy(r => r.NewSkillId);

                foreach (var skillGroup in skillGroups)
                {
                    // Get all subskills for each skill
                    var subSkills = skillGroup
                        .Where(r => r.NewSubSkillId != 0) // Ensure no invalid subskills
                        .Select(r => new SubSkillModel
                        {
                            SubSkillId = r.NewSubSkillId,
                            SubSkillName = r.SubSkillName,
                            SubSkillExperience = r.SubSkillExperience,
                            SubSkillVersion = r.SubSkillVersion,
                            SubSkillDescription = r.SubSkillDescription,
                        }).ToList();

                    var skillModels = new SkillModel
                    {
                        SkillId = skillGroup.Key,
                        SkillName = skillGroup.First().SkillName,
                        SkillExperience = skillGroup.First().SkillExperience,
                        SkillVersion = skillGroup.First().SkillVersion,
                        SkillDescription = skillGroup.First().SkillDescription,
                        SubSkills = subSkills // Add the subskills for each skill
                    };

                    skills.Add(skillModels);
                }

                // Create the FetchResourceSkill object for each resource
                var fetchResourceSkill = new FetchResourceSkill
                {
                    ResourceId = group.Key,
                    ResourceName = group.First().ResourceName,
                    DateOfJoin = group.First().DateOfJoin,
                    TotalYears = group.First().TotalYears,
                    Skills = skills // Add the skills (with subskills) for the resource
                };

                finalResult.Add(fetchResourceSkill);
            }

            return finalResult;
        }

        public async Task<List<FetchResourceSkill>> CheckResourceSkillsUpdated(int resourceId)
        {
            var currentDate = DateTime.Now;

            // Calculate the current quarter (Q1, Q2, Q3, Q4)
            var currentQuarter = (currentDate.Month - 1) / 3 + 1;
            var currentYear = currentDate.Year;

            // Calculate start and end dates for the current quarter
            var startOfQuarter = new DateTime(currentYear, (currentQuarter - 1) * 3 + 1, 1);
            var endOfQuarter = new DateTime(currentYear, currentQuarter * 3 + 1, 1).AddDays(-1); // End date of the current quarter

            // Combine query to fetch resource skills and the resource name in a single query
            var resourceWithSkills = _dbContext.ResourceSkills
                .Where(r => r.ResourceId == resourceId
                         && ((r.CreateDate >= startOfQuarter && r.CreateDate <= endOfQuarter)
                              || (r.UpdateDate >= startOfQuarter && r.UpdateDate <= endOfQuarter))
                         && r.IsActive == 1)
                .Select(r => new { r.ResourceId })  // Selecting just ResourceId
                .FirstOrDefault();  // Only active skills

            // If no skills were updated in the current quarter, return an empty list
            if (resourceWithSkills == null)
            {
                return new List<FetchResourceSkill>();
            }

            // Query to get the resource name
            var resourceName = _dbContext.Resources
                .Where(r => r.ResourceId == resourceId)
                .Select(r => r.ResourceName)  // Only selecting the ResourceName
                .FirstOrDefault();

            // If resource not found, return an empty list
            if (resourceName == null)
            {
                return new List<FetchResourceSkill>();
            }

            // Create the result with updated skills for the resource
            var fetchResourceSkill = new FetchResourceSkill
            {
                ResourceId = (int)resourceWithSkills.ResourceId,
                ResourceName = resourceName
            };

            return new List<FetchResourceSkill> { fetchResourceSkill };
        }

    }

}


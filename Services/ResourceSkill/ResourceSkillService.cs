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

      
        public async Task<ResponseModel> InsertResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel)
        {
            ResponseModel model = new ResponseModel(); try
            {
                int resourceId = resourceSkillRequestModel.ResourceId;

                foreach (var category in resourceSkillRequestModel.SkillCategories)
                {
                    foreach (var skill in category.Skills)
                    {
                        if (skill.SubSkills == null || !skill.SubSkills.Any())
                        {
                            var newResourceSkill = new ResourceSkill
                            {
                                SkillId = skill.SkillId,
                                SubSkillId = null,
                                ResourceId = resourceId,
                                SkillExperience = skill.SkillExperience,
                                SkillVersion = skill.SkillVersion,
                                SkillDescription = skill.SkillDescription,
                                SubSkillExperience = null,
                                IsActive = (int)Status.IS_ACTIVE,
                                IsDeleted = 0,
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
                                    CreateDate = DateTime.Now,
                                    RejectedBy = 0,
                                    RejectedComment = null,
                                    IsApproved=0,
                                    ApprovedBy = 0,
                                    IsDeleted = 0
                                    
                                };
                                _dbContext.ResourceSkills.Add(newResourceSkill);
                            }
                        }
                    }
                }

                await _dbContext.SaveChangesAsync();

                model.IsSuccess = true;
                model.Messsage = "Resource skill and subskills inserted successfully.";
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error: " + ex.Message;
            }
            return model;
        }
                   
        public async Task<ResponseModel> UpdateResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                int resourceId = resourceSkillRequestModel.ResourceId;

                foreach (var category in resourceSkillRequestModel.SkillCategories)
                {
                    foreach (var skill in category.Skills)
                    {
                        bool hasSubSkills = skill.SubSkills != null && skill.SubSkills.Any();

                        if (hasSubSkills)
                        {
                            foreach (var subSkill in skill.SubSkills)
                            {
                                var existingSkill = _dbContext.ResourceSkills
                                    .FirstOrDefault(rs => rs.ResourceId == resourceId && rs.SkillId == skill.SkillId && rs.IsActive == 1);

                                if (existingSkill != null && existingSkill.SubSkillId == null)
                                {
                                    // Update main skill to include this new subskill
                                    existingSkill.SubSkillId = subSkill.SubSkillId;
                                    existingSkill.SubSkillExperience = subSkill.SubSkillExperience;
                                    existingSkill.SubSkillVersion = subSkill.SubSkillVersion;
                                    existingSkill.SubSkillDescription = subSkill.SubSkillDescription;
                                    existingSkill.UpdateBy = resourceSkillRequestModel.CreateBy;
                                    existingSkill.UpdateDate = DateTime.Now;
                                    existingSkill.RejectedBy = 0;
                                    existingSkill.RejectedComment = null;
                                    existingSkill.IsApproved = 0;
                                    existingSkill.ApprovedBy = 0;

                                    _dbContext.ResourceSkills.Update(existingSkill);
                                }
                                else
                                {
                                    // Check if subskill already exists
                                    var existingSubSkill = _dbContext.ResourceSkills
                                        .FirstOrDefault(rs => rs.ResourceId == resourceId
                                            && rs.SkillId == skill.SkillId
                                            && rs.SubSkillId == subSkill.SubSkillId && rs.IsActive == 1 && rs.IsDeleted == 0);

                                    if (existingSubSkill != null)
                                    {
                                        // Update existing subskill entry
                                        existingSubSkill.SkillExperience = skill.SkillExperience;
                                        existingSubSkill.SkillVersion = skill.SkillVersion;
                                        existingSubSkill.SkillDescription = skill.SkillDescription;
                                        existingSubSkill.SubSkillExperience = subSkill.SubSkillExperience;
                                        existingSubSkill.SubSkillVersion = subSkill.SubSkillVersion;
                                        existingSubSkill.SubSkillDescription = subSkill.SubSkillDescription;
                                        existingSubSkill.IsActive = (int)Status.IS_ACTIVE;
                                        existingSubSkill.UpdateBy = resourceSkillRequestModel.CreateBy;
                                        existingSubSkill.UpdateDate = DateTime.Now;
                                        existingSubSkill.RejectedBy = 0;
                                        existingSubSkill.RejectedComment = null;
                                        existingSubSkill.IsApproved = 0;
                                        existingSubSkill.ApprovedBy = 0;
                                        existingSkill.IsDeleted = 0;

                                        _dbContext.ResourceSkills.Update(existingSubSkill);
                                    }
                                    else
                                    {
                                        // Add new subskill entry
                                        var newSubSkill = new ResourceSkill
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
                                            CreateDate = DateTime.Now,                                            
                                            RejectedBy = 0,
                                            RejectedComment = null,
                                            IsApproved = 0,
                                            ApprovedBy = 0,
                                            IsDeleted = 0,
                                        };
                                        _dbContext.ResourceSkills.Add(newSubSkill);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Handle main skill (no subskills)
                            var existingMainSkill = _dbContext.ResourceSkills
                                .FirstOrDefault(rs => rs.ResourceId == resourceId && rs.SkillId == skill.SkillId && rs.SubSkillId == null && rs.IsActive == 1);

                            if (existingMainSkill != null)
                            {
                                // Update existing main skill
                                existingMainSkill.SkillExperience = skill.SkillExperience;
                                existingMainSkill.SkillVersion = skill.SkillVersion;
                                existingMainSkill.SkillDescription = skill.SkillDescription;
                                existingMainSkill.IsActive = (int)Status.IS_ACTIVE;
                                existingMainSkill.UpdateBy = resourceSkillRequestModel.CreateBy;
                                existingMainSkill.IsApproved = 0;
                                existingMainSkill.RejectedBy = 0;
                                existingMainSkill.ApprovedBy = 0;
                                existingMainSkill.RejectedComment = null;
                                existingMainSkill.UpdateDate = DateTime.Now;
                                

                                _dbContext.ResourceSkills.Update(existingMainSkill);
                            }
                            else
                            {
                                // Add a new main skill entry
                                var newMainSkill = new ResourceSkill
                                {
                                    SkillId = skill.SkillId,
                                    SubSkillId = null, // No subskill initially
                                    ResourceId = resourceId,
                                    SkillExperience = skill.SkillExperience,
                                    SkillVersion = skill.SkillVersion,
                                    SkillDescription = skill.SkillDescription,
                                    IsActive = (int)Status.IS_ACTIVE,
                                    CreateBy = resourceSkillRequestModel.CreateBy,
                                    CreateDate = DateTime.Now,
                                    RejectedBy = 0,
                                    RejectedComment = null,
                                    IsApproved = 0,
                                    ApprovedBy = 0
                                };
                                _dbContext.ResourceSkills.Add(newMainSkill);
                            }
                        }
                    }
                }

                await _dbContext.SaveChangesAsync(); // Commit changes

                model.IsSuccess = true;
                model.Messsage = "Resource skills and subskills updated successfully.";
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
                where rs.IsActive == (int)Status.IS_ACTIVE & rs.SkillId != 0
                select new
                {
                    r.ResourceId,
                    r.ResourceName,
                    r.TotalYears,
                    r.DateOfJoin,
                    rs.SkillExperience,
                    rs.SkillVersion,
                    rs.SkillDescription,
                    rs.SubSkillExperience,
                    rs.SubSkillVersion,
                    rs.SubSkillDescription,
                    rs.ApprovedBy,
                    rs.RejectedBy,
                    rs.IsApproved,
                    rs.RejectedComment,
                    rs.ResourceSkillId,
                    rs.IsDeleted,
                    NewSkillId = skill != null ? skill.SkillId : 0, // Ensure default value
                    SkillName = skill != null ? skill.Name : null, // Default name if null
                    NewSubSkillId = subSkill != null ? subSkill.SubSkillId : 0, // Default value
                    SubSkillName = subSkill != null ? subSkill.Name : null // Default name
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
                        .Where(r => r.NewSubSkillId != 0 && r.IsDeleted != 1)
                        .Select(r => new SubSkillModel
                        {
                            SkillId=skillGroup.Key,
                            SubSkillId = r.NewSubSkillId,
                            SubSkillName = r.SubSkillName,
                            SubSkillExperience = r.SubSkillExperience,
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
                        IsApproved = skillGroup.First().IsApproved ?? 0,
                        ApprovedBy = skillGroup.First().ApprovedBy ?? 0,
                        RejectedBy = skillGroup.First().RejectedBy ?? 0,
                        RejectedComment = skillGroup.First().RejectedComment,
                        SubSkills = subSkills
                    };

                    skills.Add(skillModel);
                }

                var fetchResourceSkill = new FetchResourceSkill
                {
                    ResourceId = group.Key,
                    ResourceSkillId= group.First().ResourceSkillId,
                    ResourceName = group.First().ResourceName,
                    TotalYears= group.First().TotalYears,
                    DateOfJoin= group.First().DateOfJoin,

                    Skills = skills
                };

                finalResult.Add(fetchResourceSkill);
            }

            return finalResult.OrderBy(r => r.ResourceName).ToList();
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
                    rs.IsApproved,
                    rs.RejectedBy,
                    rs.ApprovedBy,
                    rs.IsDeleted,
                    rs.RejectedComment,
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
                                RejectedComment= skillGroup.First().RejectedComment,
                                RejectedBy = skillGroup.First().RejectedBy ?? 0,
                                IsApproved = skillGroup.First().IsApproved ?? 0,
                                ApprovedBy = skillGroup.First().ApprovedBy ?? 0,                              
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
                                        IsDeleted = r.IsDeleted
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
            var query = from rs in _dbContext.ResourceSkills.Where(rs => rs.IsActive == 1)
                        join r in _dbContext.Resources on rs.ResourceId equals r.ResourceId
                        join s in _dbContext.Skills.Where(s=> s.IsActive == 1) on rs.SkillId equals s.SkillId into skillGroup
                        from skill in skillGroup.DefaultIfEmpty()
                        join sub in _dbContext.SubSkills.Where(subskill => subskill.IsActive == 1) on rs.SubSkillId equals sub.SubSkillId into subSkillGroup
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
                            rs.IsActive,
                            rs.IsDeleted,
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
                        .Where(r => r.NewSubSkillId != 0 & r.IsDeleted ==0) // Ensure no invalid subskills
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

        //It checks if a resource has updated skills within the current quarter, fetching the resource name and returning the updated skills, or an empty list if no updates are found.
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
            var resourceWithSkills = await _dbContext.ResourceSkills
                .Where(r => r.ResourceId == resourceId
                         && ((r.CreateDate >= startOfQuarter && r.CreateDate <= endOfQuarter)
                              || (r.UpdateDate >= startOfQuarter && r.UpdateDate <= endOfQuarter))
                         && r.IsActive == 1)
                .Select(r => new { r.ResourceId })  // Selecting just ResourceId
                .FirstOrDefaultAsync();  // Only active skills

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

        //It updates the approval status for resources' skills based on the provided approval updates, saving changes to the database for each update.
        public async Task<ResponseModel> UpdateApprovalStatus(UpdateApprovalStatusRequestModel updateApproval)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                // Loop through each resource
                foreach (var resource in updateApproval.Resources)
                {
                    // Loop through each approval update for the current resource
                    foreach (var approvalUpdate in resource.ApprovalUpdates)
                    {
                        // Query using ResourceId and SkillId (and optionally SubSkillId if provided)
                        var resourceSkill = await _dbContext.ResourceSkills
                            .FirstOrDefaultAsync(rs => rs.ResourceId == resource.ResourceId && rs.IsActive == 1 && rs.SkillId == approvalUpdate.SkillId);

                        if (resourceSkill != null)
                        {
                            // Update the resource skill approval status
                            resourceSkill.IsApproved = approvalUpdate.IsApproved;
                            resourceSkill.ApprovedBy = approvalUpdate.ApprovedBy;
                            resourceSkill.RejectedBy = approvalUpdate.RejectedBy;
                            resourceSkill.RejectedComment = approvalUpdate.RejectedComment ?? string.Empty; // Handle null rejected comment
                            resourceSkill.UpdateDate = DateTime.Now;

                            // Save changes for each update
                            await _dbContext.SaveChangesAsync();
                        }
                      
                    }
                }

                // If all updates were successful, set success message
                model.IsSuccess = true;
                model.Messsage = "Approval status updated successfully.";
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error: " + ex.Message;
            }

            return model;
        }

        //It marks a resource's skills and sub-skills as inactive, resets their approval/rejection status, and updates the database
        public async Task<ResponseModel> MarkResourceSkillAsInactive(ResourceSkillRequestModel resourceSkillRequestModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                int resourceId = resourceSkillRequestModel.ResourceId;

                // Iterate through categories
                foreach (var category in resourceSkillRequestModel.SkillCategories)
                {
                    // Iterate through skills in the category
                    foreach (var skill in category.Skills)
                    {
                        bool hasSubSkills = skill.SubSkills != null && skill.SubSkills.Any();

                        // Mark sub-skills as inactive
                        if (hasSubSkills)
                        {
                            foreach (var subSkill in skill.SubSkills)
                            {
                                var existingSubSkill = _dbContext.ResourceSkills
                                    .FirstOrDefault(rs => rs.ResourceId == resourceId
                                                         && rs.SkillId == skill.SkillId
                                                         && rs.SubSkillId == subSkill.SubSkillId
                                                         && rs.IsActive == 1
                                                         && (rs.IsDeleted == 0 || rs.IsDeleted == null));

                                if (existingSubSkill != null)
                                {
                                    // 1. Update approval/rejection fields for all entries of the skill (including those with no subskill)
                                    var relatedSkills = _dbContext.ResourceSkills
                                        .Where(rs => rs.ResourceId == resourceId && rs.SkillId == skill.SkillId && rs.IsActive == 1 && rs.IsDeleted == 0)
                                        .ToList();

                                    foreach (var relatedSkill in relatedSkills)
                                    {
                                        relatedSkill.ApprovedBy = 0;
                                        relatedSkill.RejectedBy = 0;
                                        relatedSkill.RejectedComment = null;
                                        relatedSkill.IsApproved = 0;

                                        _dbContext.ResourceSkills.Update(relatedSkill);  // Update related entries
                                    }

                                    // 2. Now delete the subskill itself (mark it as deleted)
                                    existingSubSkill.IsDeleted = 1;  // Mark subskill as inactive
                                    existingSubSkill.UpdateBy = resourceSkillRequestModel.CreateBy;
                                    existingSubSkill.UpdateDate = DateTime.Now;

                                    _dbContext.ResourceSkills.Update(existingSubSkill);  // Update subskill entry
                                }
                            }
                        }
                        else
                        {
                            // No sub-skills, mark the main skill as inactive
                            var existingMainSkills = _dbContext.ResourceSkills
                                .Where(rs => rs.ResourceId == resourceId && rs.SkillId == skill.SkillId && rs.IsActive == 1)
                                .ToList();

                            foreach (var existingMainSkill in existingMainSkills)
                            {
                                // Reset approval/rejection status for the main skill
                                existingMainSkill.ApprovedBy = 0;
                                existingMainSkill.RejectedBy = 0;
                                existingMainSkill.RejectedComment = null;
                                existingMainSkill.IsApproved = 0;

                                // Mark as inactive
                                existingMainSkill.IsActive = 0;
                                existingMainSkill.UpdateBy = resourceSkillRequestModel.CreateBy;
                                existingMainSkill.UpdateDate = DateTime.Now;

                                _dbContext.ResourceSkills.Update(existingMainSkill);  // Update main skill entry
                            }
                        }
                    }
                }

                // Save changes once after all updates
                await _dbContext.SaveChangesAsync(); // Commit changes

                model.IsSuccess = true;
                model.Messsage = "Resource skills and subskills marked as inactive successfully.";
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error: " + ex.Message;
            }
            return model;
        }

    }
}
    

        

    


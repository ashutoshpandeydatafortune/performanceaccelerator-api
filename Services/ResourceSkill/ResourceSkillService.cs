using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

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

        //public async Task<ResponseModel> CreateResourceSkill(ResourceSkillRequestModel resourceSkillModel)
        //{
        //    ResponseModel model = new ResponseModel();
        //    try
        //    {

        //        var requestSkillId = _dbContext.Skills.Where(name => name.Name == resourceSkillModel.SkillName)
        //            .Select(name => name.SkillId)
        //            .FirstOrDefault();

        //        List<int?> requestSubSkillIds = new List<int?>();

        //        foreach (var subSkillModel in resourceSkillModel.SubSkills)
        //        {
        //            var subSkillId = _dbContext.SubSkills
        //                .Where(subSkill => subSkill.Name == subSkillModel.SubSkillName)
        //                .Select(subSkill => subSkill.SubSkillId)
        //                .FirstOrDefault();

        //            requestSubSkillIds.Add(subSkillId);
        //        }
        //        //Check if the resource skill already exists
        //        var resourceSkill = _dbContext.ResourceSkills.FirstOrDefault(rs => rs.SkillId == requestSkillId && rs.ResourceId == (double)resourceSkillModel.ResourceId  && rs.IsActive == 1);
        //        if (resourceSkill == null)
        //        {
        //            // Create a new resource skill if it doesn't exist
        //            resourceSkill = new ResourceSkill
        //            {
        //                SkillId = requestSkillId,
        //                ResourceId = resourceSkillModel.ResourceId,
        //                Experience = resourceSkillModel.Experience,
        //                IsActive = 1,
        //                CreateBy = resourceSkillModel.CreateBy,
        //                CreateDate = DateTime.Now
        //            };

        //            _dbContext.ResourceSkills.Add(resourceSkill);
        //        }
        //        _dbContext.ResourceSkills.RemoveRange(_dbContext.ResourceSkills.Where(rs => rs.ResourceSkillId == resourceSkill.ResourceSkillId));
        //        _dbContext.ResourceSkills.Remove(resourceSkill);
        //        foreach (var subSkillModel in requestSubSkillIds)
        //        {
        //            var subSkill = await _dbContext.SubSkills.FindAsync(subSkillModel);
        //            if (subSkill != null)
        //            {
        //                ResourceSkill skill = new ResourceSkill
        //                {
        //                    SkillId = requestSkillId,
        //                    SubSkillId = subSkill.SubSkillId,
        //                    ResourceId = resourceSkill.ResourceId,
        //                    Experience = resourceSkillModel.Experience,
        //                    IsActive = 1,
        //                    CreateBy = resourceSkill.CreateBy,
        //                    CreateDate = DateTime.Now
        //                };
        //                _dbContext.ResourceSkills.Add(skill);
        //            }
        //        }

        //        await _dbContext.SaveChangesAsync();

        //        model.IsSuccess = true;
        //        model.Messsage = "Resource skill and subskills saved successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        model.IsSuccess = false;
        //        model.Messsage = "Error: " + ex.Message;
        //    }
        //    return model;
        //}

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

                foreach (var skill in resourceSkillRequestModel.Skills)
                {
                    if (skill.SubSkills == null || !skill.SubSkills.Any())
                    {
                        // No sub-skills, add the skill with default/null sub-skill values
                        var newResourceSkill = new ResourceSkill
                        {
                            SkillId = skill.SkillId,
                            SubSkillId = null, // Assuming SubSkillId can be nullable, set to default/null
                            ResourceId = resourceId,
                            Experience = resourceSkillRequestModel.Experience,
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
                                Experience = resourceSkillRequestModel.Experience,
                                IsActive = resourceSkillRequestModel.IsActive,
                                CreateBy = resourceSkillRequestModel.CreateBy,
                                CreateDate = DateTime.Now
                            };
                            _dbContext.ResourceSkills.Add(newResourceSkill);
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


        //public async Task<ResponseModel> UpdateResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel)
        //{
        //    ResponseModel model = new ResponseModel();
        //    try
        //    {
        //        var requestSkillId = _dbContext.Skills.Where(name => name.Name == resourceSkillRequestModel.SkillName)
        //            .Select(name => name.SkillId)
        //            .FirstOrDefault();

        //        List<int?> requestSubSkillIds = new List<int?>();

        //        foreach (var subSkillModel in resourceSkillRequestModel.SubSkills)
        //        {
        //            var subSkillId = _dbContext.SubSkills
        //                .Where(subSkill => subSkill.Name == subSkillModel.SubSkillName)
        //                .Select(subSkill => subSkill.SubSkillId)
        //                .FirstOrDefault();

        //            requestSubSkillIds.Add(subSkillId);
        //        }


        //        var existingResourceSkills = _dbContext.ResourceSkills
        //            .Where(rs => rs.ResourceId == (double)resourceSkillRequestModel.ResourceId &&
        //                         rs.SkillId == requestSkillId &&
        //                         rs.IsActive == 1)
        //            .ToList();

        //        foreach (var existingResourceSkill in existingResourceSkills)
        //        {
        //            _dbContext.ResourceSkills.RemoveRange(existingResourceSkill);

        //        }

        //        // Add each subskill to the resource skill
        //        foreach (var subSkillModel in requestSubSkillIds)
        //        {
        //            var subSkill = await _dbContext.SubSkills.FindAsync(subSkillModel);
        //            if (subSkill != null)
        //            {
        //                ResourceSkill skill = new ResourceSkill
        //                {
        //                    SkillId = requestSkillId,
        //                    SubSkillId = subSkill.SubSkillId,
        //                    ResourceId = resourceSkillRequestModel.ResourceId,
        //                    Experience = resourceSkillRequestModel.Experience,
        //                    IsActive = 1,
        //                    CreateBy = resourceSkillRequestModel.CreateBy,
        //                    CreateDate = DateTime.Now
        //                };
        //                _dbContext.ResourceSkills.Add(skill);
        //            }
        //        }

        //        await _dbContext.SaveChangesAsync();

        //        model.IsSuccess = true;
        //        model.Messsage = "Resource skill and subskills saved successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        model.IsSuccess = false;
        //        model.Messsage = "Error: " + ex.Message;
        //    }
        //    return model;
        //}

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

        public async Task<List<FetchResourceSkill>> GetResourceSkillsById(int resourceId)
        {
            var result = await (
                from rs in _dbContext.ResourceSkills
                join r in _dbContext.Resources on rs.ResourceId equals r.ResourceId
                join s in _dbContext.Skills on rs.SkillId equals s.SkillId into skillGroup
                from skill in skillGroup.DefaultIfEmpty()
                join sub in _dbContext.SubSkills on rs.SubSkillId equals sub.SubSkillId into subSkillGroup
                from subSkill in subSkillGroup.DefaultIfEmpty()
                where rs.IsActive == 1 && r.ResourceId == resourceId
                select new
                {
                    r.ResourceId,
                    r.ResourceName,
                    NewSkillId = (int?)skill.SkillId,
                    SkillName = skill.Name,
                    NewSubSkillId = (int?)subSkill.SubSkillId,
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
                var skill = new List<SkillModel>();

                // Group skills and subskills
                var skillGroups = group.GroupBy(r => r.NewSkillId);

                foreach (var skillGroup in skillGroups)
                {
                    var subSkill = skillGroup
                        .Where(r => r.NewSubSkillId != 0)
                        .Select(r => new SubSkillModel
                        {
                            SkillId = r.NewSkillId,
                            SubSkillId = r.NewSubSkillId,
                            SubSkillName = r.SubSkillName
                        }).ToList();

                    var skillModel = new SkillModel
                    {
                        SkillId = skillGroup.Key.HasValue ? skillGroup.Key.Value : 0,
                        SkillName = skillGroup.First().SkillName,
                        SubSkills = subSkill
                    };

                    skill.Add(skillModel);
                }

                var fetchResourceSkill = new FetchResourceSkill
                {
                    ResourceId = group.Key,
                    ResourceName = group.First().ResourceName,
                    Skills = skill
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


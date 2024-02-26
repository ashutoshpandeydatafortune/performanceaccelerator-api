using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Resources;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Data.Entity.Migrations.Infrastructure;

namespace DF_EvolutionAPI.Services
{
    public class ResourceService : IResourceService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public ResourceService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<Resource>> GetAllResources()
        {
            return await _dbcontext.Resources.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<Resource> GetResourceByEmailId(string emailId)
        {
            Resource resource;

            try
            {
                resource = await (
                    from r in _dbcontext.Resources
                    join designation in _dbcontext.Designations on r.DesignationId equals designation.DesignationId
                    where r.EmailId == emailId
                    select new Resource
                    {
                        FunctionId = r.FunctionId,
                        ResourceId = r.ResourceId,
                        ReportingTo = r.ReportingTo,
                        ResourceName = r.ResourceName,
                        DesignationId = designation.DesignationId,
                        DesignationName = designation.DesignationName,
                    }
                ).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return resource;
        }

        public async Task<List<Resource>> GetAllResourceDetailsByResourceId(int? resourceId)
        {
            var resources = new List<Resource>();

            try
            {
                resources = await (
                    from r in _dbcontext.Resources.Where(x => x.ResourceId == resourceId && x.IsActive == 1)
                    let projectResources = (_dbcontext.ProjectResources.Where(pr => (int?)pr.ResourceId == r.ResourceId && pr.IsActive == 1)).ToList()
                    let reportingToResource = _dbcontext.Resources.FirstOrDefault(rt => rt.ResourceId == r.ReportingTo)
                    select new Resource
                    {
                        ResourceId = r.ResourceId,
                        Address = r.Address,
                        AlternetNumber = r.AlternetNumber,
                        CityId = r.CityId,
                        ContactNumber = r.ContactNumber,
                        CountryId = r.CountryId,
                        Zip = r.Zip,
                        DateOfBirth = r.DateOfBirth,
                        DateOfJoin = r.DateOfJoin,
                        DesignationId = r.DesignationId,
                        DesignationName = r.DesignationName,
                        EmailId = r.EmailId,
                        EmployeeId = r.EmployeeId,
                        IsActive = r.IsActive,
                        Primaryskill = r.Primaryskill,
                        ReportingTo = r.ReportingTo,
                        ResourceName = r.ResourceName,
                        Secondaryskill = r.Secondaryskill,
                        StateId = r.StateId,
                        StatusId = r.StatusId,
                        Strengths = r.Strengths,
                        TechCategoryId = r.TechCategoryId,
                        TenureInMonths = r.TenureInMonths,
                        TenureInYears = r.TenureInYears,
                        TotalYears = r.TotalYears,
                        YearBucket = r.YearBucket,
                        CreateBy = r.CreateBy,
                        UpdateBy = r.UpdateBy,
                        CreateDate = r.CreateDate,
                        UpdateDate = r.UpdateDate,
                        FunctionId = r.FunctionId, // Added Function id on 22-02-24
                        ResourceProjectList = projectResources,
                        ReporterName = reportingToResource.ResourceName,
                    }).ToListAsync();

                foreach (var resource in resources)
                {
                    resource.ProjectList = await GetProjects(resource);
                    resource.ClientList = await GetClients(resource);
                    resource.BusinessUnits = await GetBusinessUnits(resource);
                }
                //Added Reporting to name
                // var Reportingto = from resources in _dbcontext.Resources

            }
            catch (Exception)
            {
                throw;
            }

            return resources;
        }

        private async Task<List<Project>> GetProjects(Resource resource)
        {
            var projectList = new List<Project>();

            foreach (var rp in resource.ResourceProjectList)
            {
                var project = await (from p in _dbcontext.Projects
                                     where p.ProjectId == rp.ProjectId && p.IsActive == 1
                                     select p).FirstAsync();

                projectList.Add(project);
            }

            return projectList;
        }

        private async Task<List<Client>> GetClients(Resource resource)
        {
            var clientList = new List<Client>();

            foreach (var projectResource in resource.ProjectList)
            {
                var client = await (from c in _dbcontext.Clients
                                    where c.ClientId == projectResource.ClientId && c.IsActive == 1
                                    select c).FirstAsync();

                clientList.Add(client);
            }

            return clientList;
        }

        private async Task<List<BusinessUnit>> GetBusinessUnits(Resource resource)
        {
            var businessUnits = new List<BusinessUnit>();

            foreach (var c in resource.ClientList)
            {
                var businessUnit = await (from b in _dbcontext.BusinessUnits
                                          where b.BusinessUnitId == c.BusinessUnitId && b.IsActive == 1
                                          select b).FirstAsync();

                businessUnits.Add(businessUnit);
            }

            return businessUnits;
        }

        public async Task<string> GetChildResources(string userName)
        {
            var resources = await (
                    from resource in _dbcontext.Resources
                    join designation in _dbcontext.Designations on resource.DesignationId equals designation.DesignationId
                    select new Team
                    {
                        EmailId = resource.EmailId,
                        EmployeeId = resource.EmployeeId,
                        ResourceId = resource.ResourceId,
                        ReportingTo = resource.ReportingTo,
                        PrimarySkill = resource.Primaryskill,
                        ResourceName = resource.ResourceName,
                        DesignationId = resource.DesignationId,
                        DesignationName = designation.DesignationName
                    }
                ).ToListAsync();

            var currentUser = resources.Where(r => r.EmailId == userName);
            var someUser = _dbcontext.Resources.Where(r => r.EmailId == userName).FirstOrDefault();

            var tree = currentUser.Select(resource => new
            {
                Resource = resource,
                Children = BuildTree(resources, resource.ResourceId)
            });

            // Convert to JSON
            return JsonConvert.SerializeObject(tree, Formatting.Indented);
        }

        private List<object> BuildTree(List<Team> resources, int parentId)
        {
            var children = resources
                .Where(c => c.ReportingTo.HasValue && c.ReportingTo == parentId)
                .Select(resource => new
                {
                    Resource = resource,
                    Children = BuildTree(resources, resource.ResourceId)
                })
                .ToList<object>();

            return children.Any() ? children.Cast<object>().ToList() : null;
        }

        // For displaying the profile details
        public async Task<Resource> GetProfileDetails(int? resourceId)
        {
            var resources = new Resource();
            try
            {
                resources = await (
                   from resource in _dbcontext.Resources
                   join designation in _dbcontext.Designations on resource.DesignationId equals designation.DesignationId
                   join resourcefunction in _dbcontext.ResourceFunctions on resource.ResourcefunctionId equals resourcefunction.ResourceFunctionId
                   join reportingName in _dbcontext.Resources on resource.ResourceId equals reportingName.ReportingTo
                   where resource.ResourceId == resourceId && resource.IsActive == 1
                   select new Resource
                   {
                       ResourceId = resource.ResourceId,
                       ResourceName = resource.ResourceName,
                       EmailId = resource.EmailId,
                       EmployeeId = resource.EmployeeId,
                       ReportingToName = reportingName.ResourceName,
                       Function = resourcefunction.ResourceFunctionName,
                       Designation = designation.DesignationName,
                       TotalYears = resource.TotalYears,

                   }).FirstOrDefaultAsync();

                return resources;
            }
            catch (Exception)
            {
                throw;
            }

        }

        //For displaying the team members details

        public async Task<string> GetMyTeamDetails(int userId)
        {
            var resources = await (
                    from resource in _dbcontext.Resources
                    join designation in _dbcontext.Designations on resource.DesignationId equals designation.DesignationId
                    select new TeamDetails
                    {
                       
                        EmailId = resource.EmailId,
                        Experience = resource.TotalYears,
                        ResourceId = resource.ResourceId,
                        ReportingTo = resource.ReportingTo,
                        ResourceName = resource.ResourceName,
                        DesignationName = designation.DesignationName,
                    }
                ).ToListAsync();

            var currentUser = resources.Where(r => r.ReportingTo == userId);
            var someUser = _dbcontext.Resources.Where(r => r.ResourceId == userId).FirstOrDefault();

            var currentQuarter = await _dbcontext.QuarterDetails.FirstOrDefaultAsync(quarter => quarter.Id == 1);
            foreach (var resource in currentUser)
            {
                var userKraScoreYear = await GetUserKraScoreYear(resource.ResourceId);
                resource.AverageScoreYear = userKraScoreYear.Select(r => r.Rating).FirstOrDefault();
                var userKraScoreCurrent = GetUserKraScoreCurrent(resource.ResourceId, currentQuarter.Id);
                resource.AverageScoreCurrent = userKraScoreCurrent.Select(r => r.Rating).FirstOrDefault();
                resource.PendingEvaluation = GetNotApprovedKras((int)resource.ResourceId, currentQuarter.Id);
            }

            var tree = currentUser.Select(resource => new
            {
                Resource = resource,
            });

            // Convert to JSON
            return JsonConvert.SerializeObject(tree, Formatting.Indented);
        }

        public int GetNotApprovedKras(int userId, int quarterId)
        {
            var count = (
                from resource in _dbcontext.Resources
                join designation in _dbcontext.Designations
                on resource.DesignationId equals designation.DesignationId
                join userKras in _dbcontext.UserKRA
                on resource.ResourceId equals userKras.UserId
                where userKras.UserId == userId && userKras.QuarterId == quarterId && userKras.FinalRating == null
                select resource
            ).Count();

            return count;
        }
        public async Task<List<UserKRARatingLists>> GetUserKraScoreYear(int userId)
        {
            try
            {
                var rating = (
                    from p in _dbcontext.Projects
                    join pro in _dbcontext.ProjectResources on p.ProjectId equals pro.ProjectId
                    join resources in _dbcontext.Resources on pro.ResourceId equals resources.ResourceId
                    join userKRA in _dbcontext.UserKRA on resources.ResourceId equals userKRA.UserId
                    join quarterDetail in _dbcontext.QuarterDetails on userKRA.QuarterId equals quarterDetail.Id
                    join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                    join d in _dbcontext.Designations on resources.DesignationId equals d.DesignationId
                    where resources.ResourceId == userId && quarterDetail.IsActive == 1
                    select new
                    {
                        kraLibrary.Weightage,
                        Score = userKRA.FinalRating * kraLibrary.Weightage,
                        quarterDetail.QuarterYearRange
                    }
                ).ToList();

                var result = rating.GroupBy(r => r.QuarterYearRange)
                    .Select(group => new UserKRARatingLists
                    {
                        QuarterYearRange = group.Key,
                        Score = (double)group.Sum(r => r.Score),
                        Rating = (double)Math.Round((decimal)(group.Sum(r => r.Score) / group.Sum(r => r.Weightage)), 2)
                    })
                    .OrderByDescending(r => r.QuarterYearRange)
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<UserKRARatingLists> GetUserKraScoreCurrent(int userId, int currentQuarter)
        {
            try
            {
                var rating = (
                    from p in _dbcontext.Projects
                    join pro in _dbcontext.ProjectResources on p.ProjectId equals pro.ProjectId
                    join resources in _dbcontext.Resources on pro.ResourceId equals resources.ResourceId
                    join userKRA in _dbcontext.UserKRA on resources.ResourceId equals userKRA.UserId
                    join quarterDetail in _dbcontext.QuarterDetails on userKRA.QuarterId equals quarterDetail.Id
                    join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                    join d in _dbcontext.Designations on resources.DesignationId equals d.DesignationId
                    where resources.ResourceId == userId && quarterDetail.Id == currentQuarter && quarterDetail.IsActive == 1
                    select new
                    {
                        kraLibrary.Weightage,
                        Score = userKRA.FinalRating * kraLibrary.Weightage,
                        quarterDetail.QuarterYearRange
                    }
                    ).ToList();

                var result = rating.GroupBy(r => r.QuarterYearRange)
                          .Select(group => new UserKRARatingLists
                          {
                              QuarterYearRange = group.Key,
                              Score = (double)group.Sum(r => r.Score),
                              Rating = (double)Math.Round((decimal)(group.Sum(r => r.Score) / group.Sum(r => r.Weightage)), 2)
                          })
                          .OrderByDescending(r => r.QuarterYearRange)
                          .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

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
                                    where c.ClientId == projectResource.ClientId// && c.IsActive == 1
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
                   join reportingName in _dbcontext.Resources on resource.ReportingTo equals reportingName.ResourceId
                   join designation in _dbcontext.Designations on resource.DesignationId equals designation.DesignationId
                   join resourcefunction in _dbcontext.TechFunctions on resource.FunctionId equals resourcefunction.FunctionId
                   where resource.ResourceId == resourceId && resource.IsActive == 1
                   select new Resource
                   {
                       ResourceId = resource.ResourceId,
                       ResourceName = resource.ResourceName,
                       EmailId = resource.EmailId,
                       EmployeeId = resource.EmployeeId,
                       ReporterName = reportingName.ResourceName,
                       Function = resourcefunction.FunctionName,
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

        //Displaying the team members details
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
            //var someUser = _dbcontext.Resources.Where(r => r.ResourceId == userId).FirstOrDefault();
            var result = _dbcontext.Resources.Where(r => r.ResourceId == userId);
            //var someUser = (int)result != null ? result : 0;

            var currentQuarter = await _dbcontext.QuarterDetails.FirstOrDefaultAsync(quarter => quarter.Id == 1);
            foreach (var resource in currentUser)
            {
                var userKraScoreYear = await GetUserKraScoreYear(resource.ResourceId,currentQuarter.QuarterYearRange);
                resource.AverageScoreYear = userKraScoreYear.Select(r => r.Rating).FirstOrDefault();
                var userKraScoreCurrent = GetUserKraScoreCurrent(resource.ResourceId, currentQuarter.Id, currentQuarter.QuarterYearRange);
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

        //Getting count fo not approved kras.
        public int GetNotApprovedKras(int userId, int quarterId)
        {
            var count = (
                from resource in _dbcontext.Resources
                join designation in _dbcontext.Designations
                on resource.DesignationId equals designation.DesignationId
                join userKras in _dbcontext.UserKRA
                on resource.ResourceId equals userKras.UserId
                where userKras.UserId == userId && userKras.QuarterId == quarterId && userKras.FinalRating == null && userKras.IsActive == 1
                select resource
            ).Count();

            return count;
        }
        //Displaying yearly rating
        public async Task<List<UserKRARatingLists>> GetUserKraScoreYear(int userId, string quarterRange)
        {
            try
            {
                
                var rating = await(
                    from resources in _dbcontext.Resources
                    join userKRA in _dbcontext.UserKRA on resources.ResourceId equals userKRA.UserId
                    join quarterDetail in _dbcontext.QuarterDetails on userKRA.QuarterId equals quarterDetail.Id
                    join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                    join designation in _dbcontext.Designations on resources.DesignationId equals designation.DesignationId
                    where (resources.ResourceId == userId && quarterDetail.QuarterYearRange == quarterRange && quarterDetail.IsActive == 1 && userKRA.IsActive == 1 && userKRA.FinalComment != null)
                    group new { kraLibrary, userKRA, quarterDetail } by new { quarterDetail.QuarterYear, quarterDetail.QuarterYearRange, quarterDetail.Id, quarterDetail.QuarterName, } into grouped
                    select new
                    {
                        grouped.Key.Id,
                        grouped.Key.QuarterName,
                        grouped.Key.QuarterYear,
                        grouped.Key.QuarterYearRange,
                        Weightage = grouped.Sum(x => x.kraLibrary.Weightage),
                        Score = grouped.Sum(x => x.userKRA.FinalRating * x.kraLibrary.Weightage)
                    }
                    ).ToListAsync();

                var results = rating.Select(r => new UserKRARatingList
                {
                    QuarterYearRange = r.QuarterYearRange,
                    QuarterName = r.QuarterName,
                    Rating = Math.Round((double)r.Score / (double)r.Weightage, 2)
                })
                    .OrderBy(quarter => quarter.QuarterYearRange)
                    .ToList();
                var averageRating = results?.Any() == true ? Math.Round((double)results.Average(average => average.Rating), 2) : 0.0;
                var resultList = new List<UserKRARatingLists>
        {
            new UserKRARatingLists { Rating = (double)averageRating }
        };

                return resultList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Displaying rating od current for quarter 'Jan-Mar'.
        public List<UserKRARatingLists> GetUserKraScoreCurrent(int userId, int currentQuarter, string quarterRange)
        {
            try
            {
                var rating = (
                    from resources in _dbcontext.Resources
                    join userKRA in _dbcontext.UserKRA on resources.ResourceId equals userKRA.UserId
                    join quarterDetail in _dbcontext.QuarterDetails on userKRA.QuarterId equals quarterDetail.Id
                    join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                    join designation in _dbcontext.Designations on resources.DesignationId equals designation.DesignationId
                    where (resources.ResourceId == userId && quarterDetail.QuarterYearRange == quarterRange && quarterDetail.Id == currentQuarter && quarterDetail.IsActive == 1 && userKRA.IsActive == 1 && userKRA.FinalComment != null)
                    group new { kraLibrary, userKRA, quarterDetail } by new { quarterDetail.QuarterYear, quarterDetail.QuarterYearRange, quarterDetail.Id, quarterDetail.QuarterName, } into grouped
                    select new
                    {
                        grouped.Key.Id,
                        grouped.Key.QuarterName,
                        grouped.Key.QuarterYear,
                        grouped.Key.QuarterYearRange,
                        Weightage = grouped.Sum(x => x.kraLibrary.Weightage),
                        Score = grouped.Sum(x => x.userKRA.FinalRating * x.kraLibrary.Weightage)
                    }
                    ).ToList();

                var results = rating.Select(r => new UserKRARatingList
                {
                    QuarterYearRange = r.QuarterYearRange,
                    QuarterName = r.QuarterName,
                    Rating = Math.Round((double)r.Score / (double)r.Weightage, 2)
                })
                    .OrderBy(x => x.QuarterYearRange)
                    .ToList();
                var averageRating = results?.Any() == true ? Math.Round((double)results.Average(x => x.Rating), 2) : 0.0;
               // var averageRating = Math.Round((double)results.Average(x => x.Rating),2);

                var resultList = new List<UserKRARatingLists>
        {
            new UserKRARatingLists { Rating = (double)averageRating }
        };

                return resultList;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

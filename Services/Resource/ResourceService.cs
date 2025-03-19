using System;
using System.Linq;
using Newtonsoft.Json;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using DF_EvolutionAPI.Models.Response;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

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
            return await _dbcontext.Resources.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<Resource> GetResourceByEmailId(string emailId)
        {
            Resource resource;

            try
            {
                resource = await (
                    from r in _dbcontext.Resources
                    join designation in _dbcontext.Designations on r.DesignationId equals designation.DesignationId
                    where r.EmailId == emailId && r.IsActive == (int)Status.IS_ACTIVE && r.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID
                    select new Resource
                    {
                        FunctionId = r.FunctionId,
                        ResourceId = r.ResourceId,
                        ReportingTo = r.ReportingTo,
                        ResourceName = r.ResourceName,
                        DesignationId = designation.DesignationId,
                        DesignationName = designation.DesignationName,
                        IsActive = r.IsActive,
                        StatusId = r.StatusId,
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
                    from r in _dbcontext.Resources.Where(x => x.ResourceId == resourceId && x.IsActive == (int)Status.IS_ACTIVE)
                    let projectResources = (_dbcontext.ProjectResources.Where(pr => (int?)pr.ResourceId == r.ResourceId && pr.IsActive == (int)Status.IS_ACTIVE)).ToList()
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
                                     where p.ProjectId == rp.ProjectId && p.IsActive == (int)Status.IS_ACTIVE
                                     select p).FirstOrDefaultAsync();

                if (project != null)// To restrict adding a null value in the case of an inactive project
                {
                    projectList.Add(project);
                }
            }

            return projectList;
        }

        private async Task<List<Client>> GetClients(Resource resource)
        {
            var clientList = new List<Client>();

            foreach (var projectResource in resource.ProjectList)
            {
                var client = await (from c in _dbcontext.Clients
                                    where c.ClientId == projectResource.ClientId && c.IsActive == (int)Status.IS_ACTIVE
                                    select c).FirstOrDefaultAsync();
                if (client != null)// To restrict adding a null value in the case of an inactive client
                {
                    clientList.Add(client);
                }
            }

            return clientList;
        }

        private async Task<List<BusinessUnit>> GetBusinessUnits(Resource resource)
        {
            var businessUnits = new List<BusinessUnit>();

            foreach (var c in resource.ClientList)
            {
                var businessUnit = await (from b in _dbcontext.BusinessUnits
                                          where b.BusinessUnitId == c.BusinessUnitId && b.IsActive == (int)Status.IS_ACTIVE
                                          select b).FirstOrDefaultAsync();
                if (businessUnit != null)
                {
                    businessUnits.Add(businessUnit);
                }
            }

            return businessUnits;
        }

        public async Task<string> GetChildResources(string userName)
        {
            var resources = await (
                    from resource in _dbcontext.Resources
                    join designation in _dbcontext.Designations on resource.DesignationId equals designation.DesignationId
                    where resource.IsActive == (int)Status.IS_ACTIVE && resource.StatusId == 8    // Alllowing all active resources.
                    //where resource.IsActive == 1

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

        // Gets the profile details
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
                   where resource.ResourceId == resourceId && resource.IsActive == (int)Status.IS_ACTIVE
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

        // Gets the team members details
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
            var result = _dbcontext.Resources.Where(r => r.ResourceId == userId);

            var currentQuarter = await _dbcontext.QuarterDetails.FirstOrDefaultAsync(quarter => quarter.Id == 1);
            foreach (var resource in currentUser)
            {
                var userKraScoreYear = await GetUserKraScoreYear(resource.ResourceId, currentQuarter.QuarterYearRange);
                resource.AverageScoreYear = userKraScoreYear.Select(r => r.Rating).FirstOrDefault();
                var userKraScoreCurrent = GetUserKraScoreCurrent(resource.ResourceId, currentQuarter.Id, currentQuarter.QuarterYearRange);
                resource.AverageScoreCurrent = userKraScoreCurrent.Select(r => r.Rating).FirstOrDefault();
                resource.PendingEvaluation = GetNotApprovedKras((int)resource.ResourceId);

            }
            var tree = currentUser.Select(resource => new
            {
                Resource = resource,
            });

            // Convert to JSON
            return JsonConvert.SerializeObject(tree, Formatting.Indented);
        }

        // Gets the count of distinct quarters for KRAs that are not approved for a specified user.
        public int GetNotApprovedKras(int userId)
        {
            var quarterCount = (
                from resource in _dbcontext.Resources
                join designation in _dbcontext.Designations
                on resource.DesignationId equals designation.DesignationId
                join userKras in _dbcontext.UserKRA
                on resource.ResourceId equals userKras.UserId
                join quarters in _dbcontext.QuarterDetails // Join with QuarterDetails to associate user KRAs with their respective quarters.
                on userKras.QuarterId equals quarters.Id
                where userKras.UserId == userId // Filter by the specified user ID
                && userKras.FinalRating == null // Only include KRAs that have not been approved (FinalRating is null)
                && userKras.IsActive == (int)Status.IS_ACTIVE  // Ensure the KRA is active
                && userKras.DeveloperRating != null // Include only those KRAs that have a DeveloperRating
                && userKras.RejectedBy == null // Include only those kras which are not rejected.
                select quarters.Id // Select the quarter ID to count distinct quarters
            ).Distinct().Count(); // Count the distinct quarter IDs

            return quarterCount;
        }

        //Gets average yearly KRA rating for a user within the specified quarter range.
        public async Task<List<UserKRARatingLists>> GetUserKraScoreYear(int userId, string quarterRange)
        {
            try
            {

                var rating = await (
                    from resources in _dbcontext.Resources
                    join userKRA in _dbcontext.UserKRA on resources.ResourceId equals userKRA.UserId
                    join quarterDetail in _dbcontext.QuarterDetails on userKRA.QuarterId equals quarterDetail.Id
                    join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                    join designation in _dbcontext.Designations on resources.DesignationId equals designation.DesignationId
                    // Removed the filter on QuarterYearRange because ratings should include scores from the last quarter,
                    // which may include ratings from the previous year along with ratings for the new year.
                    // where (resources.ResourceId == userId && quarterDetail.QuarterYearRange == quarterRange && quarterDetail.IsActive == (int)Status.IS_ACTIVE && userKRA.IsActive == (int)Status.IS_ACTIVE && userKRA.FinalComment != null)
                    where (resources.ResourceId == userId &&  quarterDetail.IsActive == (int)Status.IS_ACTIVE && userKRA.IsActive == (int)Status.IS_ACTIVE && userKRA.IsApproved != 0)
                    group new { kraLibrary, userKRA, quarterDetail } by new { quarterDetail.QuarterYear, quarterDetail.QuarterYearRange, quarterDetail.Id, quarterDetail.QuarterName } into grouped
                    select new
                    {
                        grouped.Key.Id,
                        grouped.Key.QuarterName,
                        grouped.Key.QuarterYear,
                        grouped.Key.QuarterYearRange,                       
                        Weightage = grouped.Sum(x => x.kraLibrary.Weightage),
                        Score = grouped.Sum(x => x.userKRA.FinalRating * x.kraLibrary.Weightage)
                    }
                    )
                    .OrderBy(r=>r.QuarterYear)                   
                    .ToListAsync();

                var lastQuarterRatings = rating.TakeLast(4).ToList();// // Get the ratings for the last 4 quarters

                var results = lastQuarterRatings.Select(r => new UserKRARatingList
                {
                    QuarterYearRange = r.QuarterYearRange,
                    QuarterName = r.QuarterName,
                    Rating = Math.Round((double)r.Score / (double)r.Weightage, 2)
                })
                    .OrderBy(quarter => quarter.QuarterYearRange)
                    .ToList();

                var averageRating = results?.Any() == true ? Math.Round((double)results.Average(average => average.Rating), 2) : 0.0;
              
                if (rating.Count == 1)//If there is only 1 rating, set the count to 0 for the 1st quarter
                {
                    averageRating = 0.0;
                }

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

        // Gets the average KRA rating for a user in the specified quarter and quarter range.
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
                    where (resources.ResourceId == userId && quarterDetail.QuarterYearRange == quarterRange && quarterDetail.Id == currentQuarter && quarterDetail.IsActive == (int)Status.IS_ACTIVE && userKRA.IsActive == (int)Status.IS_ACTIVE && userKRA.IsApproved != 0)
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

        //Retrieves a list of designations associated with a specific function
        public async Task<List<FunctionsDesignations>> GetDesignationsByFunctionId(int functionId)
        {
            var result = await (from des in _dbcontext.Designations
                                join res in _dbcontext.Resources on des.DesignationId equals res.DesignationId
                                join tecfun in _dbcontext.TechFunctions on res.FunctionId equals tecfun.FunctionId
                                where tecfun.FunctionId == functionId
                                     && tecfun.IsActive == (int)Status.IS_ACTIVE
                                     && des.IsActive == (int)Status.IS_ACTIVE
                                group new { des, tecfun } by new { des.DesignationId, des.DesignationName } into grouped

                                select new FunctionsDesignations
                                {
                                    FunctionId = (int)grouped.Select(g => g.tecfun.FunctionId).FirstOrDefault(),
                                    FunctionName = grouped.Select(g => g.tecfun.FunctionName).FirstOrDefault(),
                                    DesignationId = grouped.Key.DesignationId,
                                    DesignationName = grouped.Key.DesignationName
                                }).ToListAsync();

            return result;
        }

        //Retrieves a list of designated roles associated with a specific function
        public async Task<List<FunctionsDesignations>> GetDesignatedRolesByFunctionId(int functionId)
        {
            var result = await (from des in _dbcontext.DesignatedRoles
                                join res in _dbcontext.Resources on des.DesignatedRoleId equals res.DesignatedRoleId
                                join tecfun in _dbcontext.TechFunctions on res.FunctionId equals tecfun.FunctionId
                                where tecfun.FunctionId == functionId
                                     && tecfun.IsActive == (int)Status.IS_ACTIVE
                                     && des.IsActive == (int)Status.IS_ACTIVE
                                group new { des, tecfun } by new { des.DesignatedRoleId, des.DesignatedRoleName } into grouped

                                select new FunctionsDesignations
                                {
                                    FunctionId = (int)grouped.Select(g => g.tecfun.FunctionId).FirstOrDefault(),
                                    FunctionName = grouped.Select(g => g.tecfun.FunctionName).FirstOrDefault(),
                                    DesignatedRoleId = grouped.Key.DesignatedRoleId,
                                    DesignatedRoleName = grouped.Key.DesignatedRoleName,
                                }).ToListAsync();

            return result;
        }

        // Retrieves a list of resources with KRA status for each quarter      

        //public async Task<List<ResourceKrasSatus>> GetResourcesKrasStatus(SearchKraStatus searchKraStatus)
        //{
        //    var result = await (
        //        from k in _dbcontext.KRALibrary
        //        join uk in _dbcontext.UserKRA on k.Id equals uk.KRAId
        //        join r in _dbcontext.Resources on uk.UserId equals r.ResourceId
        //        join qd in _dbcontext.QuarterDetails on uk.QuarterId equals qd.Id
        //        join des in _dbcontext.DesignatedRoles on r.DesignatedRoleId equals des.DesignatedRoleId
        //        where (searchKraStatus.FunctionId == 0 || r.FunctionId == searchKraStatus.FunctionId)
        //              && (searchKraStatus.DesignatedRoleId == 0 || r.DesignatedRoleId == searchKraStatus.DesignatedRoleId)
        //                && (searchKraStatus.FromDate == null || uk.CreateDate.Date >= searchKraStatus.FromDate)
        //      && (searchKraStatus.ToDate == null || uk.CreateDate.Date <= searchKraStatus.ToDate)
        //        select new
        //        {
        //            r.ResourceId,
        //            r.ResourceName,
        //            des.DesignatedRoleName,
        //            Quarter = $"{qd.QuarterName} {qd.QuarterYear}",
        //            qd.QuarterName,
        //            k.Name,
        //            uk.DeveloperRating,
        //            uk.ManagerRating,
        //            uk.FinalRating,
        //            uk.RejectedBy,
        //            uk.FinalComment,
        //            uk.IsApproved,
        //        })
        //        .ToListAsync();

        //    var flattenedResult = result
        //        .GroupBy(x => new
        //        {
        //            x.ResourceId,
        //            x.ResourceName,
        //            x.DesignatedRoleName
        //        })
        //        .Select(g => new ResourceKrasSatus
        //        {
        //            ResourceId = g.Key.ResourceId,
        //            ResourceName = g.Key.ResourceName,
        //            DesignatedRole = g.Key.DesignatedRoleName,
        //            Completed = g.GroupBy(x => x.Quarter).Any(q => q.All(item => item.IsApproved != 0)) ? 1 : 0, // 1 if at least one quarter has all comments not null
        //            Pending = g.GroupBy(x => x.Quarter).Count(q => q.Any(item => item.IsApproved == 0)), // Count the quarters with at least one null comment
        //            Kras = g.GroupBy(x => x.Quarter) // Use 'Kras' with a capital 'K'
        //                 .Select(q => new KraQuarter
        //                 {
        //                     Quarter = q.Key,
        //                     QuarterName = q.Select(item => item.QuarterName).FirstOrDefault(),
        //                     Ratings = q.Select(item => new KraRating
        //                     {
        //                         KraName = item.Name,
        //                         DeveloperRating = item.DeveloperRating,
        //                         ManagerRating = item.ManagerRating,
        //                         FinalRating = item.FinalRating,
        //                         RejectedBy = item.RejectedBy,
        //                         //FinalComment = item.FinalComment,
        //                         IsApproved = item.IsApproved,
        //                     }).ToList(),
        //                 }).ToList()
        //        }).ToList();

        //    return flattenedResult;
        //}

        public async Task<List<ResourceKrasSatus>> GetResourcesKrasStatus(SearchKraStatus searchKraStatus)
        {
            var result = await (
                from k in _dbcontext.KRALibrary
                join uk in _dbcontext.UserKRA on k.Id equals uk.KRAId
                join r in _dbcontext.Resources on uk.UserId equals r.ResourceId
                join qd in _dbcontext.QuarterDetails on uk.QuarterId equals qd.Id where qd.IsActive == (int)Status.IS_ACTIVE
                join des in _dbcontext.DesignatedRoles on r.DesignatedRoleId equals des.DesignatedRoleId
                // Join to get the ReportingTo (Manager) name
                join reportingToResource in _dbcontext.Resources on r.ReportingTo equals reportingToResource.ResourceId into rpt
                from manager in rpt.DefaultIfEmpty() // Left join, if no manager, it will be null
                                                     // Join to get the ReportingTo name of the Manager (second level)
                join managerReportingTo in _dbcontext.Resources on manager.ReportingTo equals managerReportingTo.ResourceId into managerRpt
                from manager2 in managerRpt.DefaultIfEmpty() // Left join, if no second level manager, it will be null
                where (searchKraStatus.FunctionId == 0 || r.FunctionId == searchKraStatus.FunctionId)
                      && (searchKraStatus.DesignatedRoleId == 0 || r.DesignatedRoleId == searchKraStatus.DesignatedRoleId)
                      && (searchKraStatus.FromDate == null || uk.CreateDate.Date >= searchKraStatus.FromDate)
                      && (searchKraStatus.ToDate == null || uk.CreateDate.Date <= searchKraStatus.ToDate)
                select new
                {
                    r.ResourceId,
                    r.ResourceName,
                    des.DesignatedRoleName,
                    Quarter = $"{qd.QuarterName} {qd.QuarterYear}",
                    qd.QuarterName,
                    k.Name,
                    uk.DeveloperRating,
                    uk.ManagerRating,
                    uk.FinalRating,
                    uk.RejectedBy,
                    uk.FinalComment,
                    uk.IsApproved,
                    ReportingToName = manager.ResourceName, // Manager's name
                    ManagerReportingToName = manager2.ResourceName // Manager's manager name
                })
                .ToListAsync();

            var flattenedResult = result
                .GroupBy(x => new
                {
                    x.ResourceId,
                    x.ResourceName,
                    x.DesignatedRoleName
                })
                .Select(g => new ResourceKrasSatus
                {
                    ResourceId = g.Key.ResourceId,
                    ResourceName = g.Key.ResourceName,
                    DesignatedRole = g.Key.DesignatedRoleName,
                    Completed = g.GroupBy(x => x.Quarter).Count(q => q.All(item => item.IsApproved != 0)), // 1 if at least one quarter has all comments not null
                    Pending = g.GroupBy(x => x.Quarter).Count(q => q.Any(item => item.IsApproved == 0)), // Count the quarters with at least one null comment
                    Kras = g.GroupBy(x => x.Quarter) // Use 'Kras' with a capital 'K'
                        .Select(q => new KraQuarter
                        {
                            Quarter = q.Key,
                            QuarterName = q.Select(item => item.QuarterName).FirstOrDefault(),
                            Ratings = q.Select(item => new KraRating
                            {
                                KraName = item.Name,
                                DeveloperRating = item.DeveloperRating,
                                ManagerRating = item.ManagerRating,
                                FinalRating = item.FinalRating,
                                RejectedBy = item.RejectedBy,
                                IsApproved = item.IsApproved,
                            }).ToList(),
                        }).ToList(),
                    ReportingToName = g.FirstOrDefault().ReportingToName, // Fetching the manager's name
                    ManagerReportingToName = g.FirstOrDefault().ManagerReportingToName // Fetching the second-level manager's name
                }).ToList();

            return flattenedResult;
        }

        public async Task<ReportingToName> GetUserManagerName(int userId)
        {
            var result = await (from r in _dbcontext.Resources
                                join manager in _dbcontext.Resources
                                on r.ReportingTo equals manager.ResourceId into mgr
                                from manager in mgr.DefaultIfEmpty() // Left Join to handle null
                                where r.ResourceId == userId // Ensure we're filtering correctly
                                select new ReportingToName
                                {
                                    ManagerName = manager != null ? manager.ResourceName : "No Manager"
                                })
                                .FirstOrDefaultAsync(); // Avoid exceptions if no result

            return result;
        }


    }
}

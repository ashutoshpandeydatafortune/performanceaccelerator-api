using System;
using System.Linq;
using Newtonsoft.Json;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using DF_EvolutionAPI.Models.Response;
using Microsoft.EntityFrameworkCore;
using DF_EvolutionAPI.Utils;
using System.Globalization;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Services
{
    public class ResourceService : IResourceService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<ResourceService> _logger;


        public ResourceService(DFEvolutionDBContext dbContext, ILogger<ResourceService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
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
                    where r.EmailId == emailId && r.IsActive == (int)Status.IS_ACTIVE && r.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID && !r.EmployeeId.StartsWith(Constant.EMPLOYEE_PREFIX)
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
                    where resource.IsActive == (int)Status.IS_ACTIVE && resource.StatusId == 8 && !resource.EmployeeId.StartsWith(Constant.EMPLOYEE_PREFIX)   // Allowing all active resources.
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
                        TenureInMonths = resource.TenureInMonths,    // Include to calculate total experience
                        DateOfJoin = resource.DateOfJoin               // Include to calculate total experience
                    }).FirstOrDefaultAsync();

                if (resources != null)
                {
                    var experience = CalculateTotalExperience((int)resources.TenureInMonths, resources.DateOfJoin);
                    resources.TotalExperienceYears = experience.Years;
                    resources.TotalExperienceMonths = experience.Months;
                }

                return resources;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }

        //Method to calculate total experience
        private (int Years, int Months) CalculateTotalExperience(int tenureInMonths, DateTime? dateOfJoin)
        {
            if (!dateOfJoin.HasValue)
                return (0, 0);

            DateTime today = DateTime.Today;
            DateTime joinDate = dateOfJoin.Value;

            int monthsSinceJoin = ((today.Year - joinDate.Year) * 12) + today.Month - joinDate.Month;

            if (today.Day < joinDate.Day)
            {
                monthsSinceJoin -= 1;
            }

            int totalMonthsExperience = tenureInMonths + monthsSinceJoin;

            int years = totalMonthsExperience / 12;
            int months = totalMonthsExperience % 12;

            return (years, months);
        }

        // Gets the team members details
        public async Task<string> GetMyTeamDetails(int userId)
        {
            var resources = await (
                    from resource in _dbcontext.Resources
                    join designation in _dbcontext.Designations on resource.DesignationId equals designation.DesignationId
                    where resource.IsActive == (int)Status.IS_ACTIVE && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID && !resource.EmployeeId.StartsWith(Constant.EMPLOYEE_PREFIX)
                    select new TeamDetails
                    {
                        EmailId = resource.EmailId,                        
                        ResourceId = resource.ResourceId,
                        ReportingTo = resource.ReportingTo,
                        ResourceName = resource.ResourceName,
                        DesignationName = designation.DesignationName,
                        TenureInMonths = (int)resource.TenureInMonths, // Add this field
                        DateOfJoin = resource.DateOfJoin,
                    }
                ).ToListAsync();

            var currentUser = resources.Where(r => r.ReportingTo == userId);
            var result = _dbcontext.Resources.Where(r => r.ResourceId == userId);

            var currentQuarter = await _dbcontext.QuarterDetails.FirstOrDefaultAsync(quarter => quarter.Id == 1);
            foreach (var resource in currentUser)
            {
                //calculating the experience.
                var experience = CalculateTotalExperience(resource.TenureInMonths, resource.DateOfJoin);
                resource.Experience = $"{experience.Years}.{experience.Months}";
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
        public async Task<List<ResourceKrasSatus>> GetResourcesKrasStatus(SearchKraStatus searchKraStatus)
        {
            try
            {
                var result = await (
                    from k in _dbcontext.KRALibrary
                    join uk in _dbcontext.UserKRA on k.Id equals uk.KRAId
                    join r in _dbcontext.Resources on uk.UserId equals r.ResourceId
                    where r.IsActive == (int)Status.IS_ACTIVE && r.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID && !r.EmployeeId.StartsWith(Constant.EMPLOYEE_PREFIX)
                    join qd in _dbcontext.QuarterDetails on uk.QuarterId equals qd.Id
                    where qd.IsActive == (int)Status.IS_ACTIVE && uk.IsActive == (int)Status.IS_ACTIVE
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
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

        public async Task<QuarterDetails> GetCurrentQuarter()
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(QuarterDetails), nameof(GetCurrentQuarter));
            try
            {
                var currentDate = DateTime.Now;
                return await _dbcontext.QuarterDetails
                    .FirstOrDefaultAsync(q =>
                        q.QuarterYear == currentDate.Year
                        && q.IsActive == 1
                        && q.IsDeleted == 0
                        && (
                            (q.QuarterName == "Jan-Mar" && currentDate.Month >= 1 && currentDate.Month <= 3) ||
                            (q.QuarterName == "Apr-Jun" && currentDate.Month >= 4 && currentDate.Month <= 6) ||
                            (q.QuarterName == "Jul-Sep" && currentDate.Month >= 7 && currentDate.Month <= 9) ||
                            (q.QuarterName == "Oct-Dec" && currentDate.Month >= 10 && currentDate.Month <= 12)
                        ));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }      


        // Gets the list of resources whose evaluation is pending by the manager.
        public async Task<ResourceEvaluationResponse> GetPendingResourceEvaluations(int? userId)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ResourceEvaluationResponse), nameof(GetPendingResourceEvaluations));
            try
            {
                _logger.LogInformation("Entering method for userId: {UserId}", userId);
                var currentQuarter = await GetCurrentQuarter();

                if (currentQuarter == null)
                {
                    _logger.LogWarning("Current quarter not found for userId: {UserId}", userId);
                    // Handle case where current quarter is not found
                    return new ResourceEvaluationResponse
                    {
                        totalCount = 0,
                        ResourceEvaluationList = new List<ResourceEvaluation>()
                    };
                }

                // Fetch raw matching data
                var rawData = await (
                    from resource in _dbcontext.Resources
                    join designatedRole in _dbcontext.DesignatedRoles
                        on resource.DesignatedRoleId equals designatedRole.DesignatedRoleId
                    join userKras in _dbcontext.UserKRA
                        on resource.ResourceId equals userKras.UserId
                    join quarters in _dbcontext.QuarterDetails
                        on userKras.QuarterId equals quarters.Id
                    where resource.ReportingTo == userId
                        && resource.IsActive == (int)Status.IS_ACTIVE
                        && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID
                        && userKras.FinalRating == null
                        && userKras.IsActive == (int)Status.IS_ACTIVE
                        && (userKras.DeveloperRating != null || userKras.RejectedBy != null)   
                        && userKras.QuarterId == currentQuarter.Id
                    select new
                    {
                        resource.ResourceId,
                        resource.ResourceName,
                        quarters.Id,
                        quarters.QuarterName
                    }
                ).ToListAsync();               
                
                // Group and format data in memory
                var resourceEvaluationList = rawData
                    .GroupBy(x => new { x.ResourceId, x.ResourceName })
                    .Select(grouped => new ResourceEvaluation
                    {
                        ResourceId = grouped.Key.ResourceId,
                        ResourceName = grouped.Key.ResourceName,
                        QuarterId = string.Join(", ", grouped.Select(q => q.Id).Distinct()),
                        QuarterName = string.Join(", ", grouped.Select(q => q.QuarterName).Distinct())
                    })
                    .ToList();
                             
                // Build and return the response
                return new ResourceEvaluationResponse
                {
                    totalCount = resourceEvaluationList.Count,
                    ResourceEvaluationList = resourceEvaluationList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                // Optionally log the exception
                return new ResourceEvaluationResponse
                {
                    totalCount = 0,
                    ResourceEvaluationList = new List<ResourceEvaluation>()
                };
            }
        }

        // Gets the list of resources whose evaluations are completed.
        public async Task<ResourceEvaluationResponse> GetCompletedResourceEvaluations(int? userId)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ResourceEvaluationResponse), nameof(GetCompletedResourceEvaluations));
            try
            {
                _logger.LogInformation("Entering method for userId: {UserId}", userId);
                var currentQuarter = await GetCurrentQuarter();

                if (currentQuarter == null)
                {
                    _logger.LogWarning("Current quarter not found for userId: {UserId}", userId);
                    // Handle case where current quarter is not found
                    return new ResourceEvaluationResponse
                    {
                        totalCount = 0,
                        ResourceEvaluationList = new List<ResourceEvaluation>()
                    };
                }

                // Fetch raw matching data
                var rawData = await (
                    from resource in _dbcontext.Resources
                    join designatedRole in _dbcontext.DesignatedRoles
                        on resource.DesignatedRoleId equals designatedRole.DesignatedRoleId
                    join userKras in _dbcontext.UserKRA
                        on resource.ResourceId equals userKras.UserId
                    join quarters in _dbcontext.QuarterDetails
                        on userKras.QuarterId equals quarters.Id
                    where resource.ReportingTo == userId
                          && resource.IsActive == (int)Status.IS_ACTIVE 
                          && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID
                          && userKras.FinalRating != 0
                          && userKras.IsActive == (int)Status.IS_ACTIVE
                          && (userKras.DeveloperRating != null)
                          && (userKras.RejectedBy == null)
                          && userKras.IsApproved == 1
                          && userKras.QuarterId == currentQuarter.Id
                    select new
                    {
                        resource.ResourceId,
                        resource.ResourceName,
                        quarters.Id,
                        quarters.QuarterName
                    }
                ).ToListAsync();
                
                // Group and format data in memory
                var resourceEvaluationList = rawData
                    .GroupBy(x => new { x.ResourceId, x.ResourceName })
                    .Select(grouped => new ResourceEvaluation
                    {
                        ResourceId = grouped.Key.ResourceId,
                        ResourceName = grouped.Key.ResourceName,
                        QuarterId = string.Join(", ", grouped.Select(q => q.Id).Distinct()),
                        QuarterName = string.Join(", ", grouped.Select(q => q.QuarterName).Distinct())
                    })
                    .ToList();
               
                // Build and return the response
                return new ResourceEvaluationResponse
                {
                    totalCount = resourceEvaluationList.Count,
                    ResourceEvaluationList = resourceEvaluationList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                // Optionally log the exception
                return new ResourceEvaluationResponse
                {
                    totalCount = 0,
                    ResourceEvaluationList = new List<ResourceEvaluation>()
                };
            }
        }

        // Gets the list of resources whose self-evaluation is pending.
        public async Task<ResourceEvaluationResponse> GetPendingSelfEvaluations (int? userId)
       {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ResourceEvaluationResponse), nameof(GetPendingSelfEvaluations));
            try
            {
                _logger.LogInformation("Entering method for userId: {UserId}", userId);
                var currentQuarter = await GetCurrentQuarter();

                if (currentQuarter == null)
                {
                    // Handle case where current quarter is not found
                    return new ResourceEvaluationResponse
                    {
                        totalCount = 0,
                        ResourceEvaluationList = new List<ResourceEvaluation>()
                    };
                }


                // Fetch raw matching data
                var rawData = await (
                    from resource in _dbcontext.Resources
                    join designatedRole in _dbcontext.DesignatedRoles
                        on resource.DesignatedRoleId equals designatedRole.DesignatedRoleId
                    join userKras in _dbcontext.UserKRA
                        on resource.ResourceId equals userKras.UserId
                    join quarters in _dbcontext.QuarterDetails
                        on userKras.QuarterId equals quarters.Id
                    where resource.ReportingTo == userId                     
                      && resource.IsActive == (int)Status.IS_ACTIVE
                      && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID
                      && userKras.FinalRating == null
                      && userKras.IsActive == (int)Status.IS_ACTIVE
                      && (userKras.DeveloperRating == null)
                      && (userKras.RejectedBy == null)
                      && userKras.IsApproved == 0
                      && userKras.QuarterId == currentQuarter.Id
                    select new
                    {
                        resource.ResourceId,
                        resource.ResourceName,
                        resource.EmailId,
                        quarters.Id,
                        quarters.QuarterName
                    }
                ).ToListAsync();
                
                // Group and format data in memory
                var resourceEvaluationList = rawData
                    .GroupBy(x => new { x.ResourceId, x.ResourceName })
                    .Select(grouped => new ResourceEvaluation
                    {
                        ResourceId = grouped.Key.ResourceId,
                        ResourceName = grouped.Key.ResourceName,
                        QuarterId = string.Join(", ", grouped.Select(q => q.Id).Distinct()),
                        QuarterName = string.Join(", ", grouped.Select(q => q.QuarterName).Distinct())
                    })
                    .ToList();
               
                // Build and return the response
                return new ResourceEvaluationResponse
                {
                    totalCount = resourceEvaluationList.Count,
                    ResourceEvaluationList = resourceEvaluationList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                // Optionally log the exception
                return new ResourceEvaluationResponse
                {
                    totalCount = 0,
                    ResourceEvaluationList = new List<ResourceEvaluation>()
                };
            }
        }
        //Get the list of the resources whoes kras final rating is given.
        public async Task<List<ApprovalResources>> GetPendingKrasApprovalResources(int userId, int quarterId)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ApprovalResources), nameof(GetPendingKrasApprovalResources));
            try
            {
                _logger.LogInformation("Entering method for userId: {UserId}, quarterId: {QuarterId}", userId, quarterId);

                var reportingIds = await _dbcontext.Resources
                    .Where(r => r.ReportingTo == userId
                                && r.IsActive == (int)Status.IS_ACTIVE
                                && r.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID)
                    .Select(r => r.ResourceId)
                    .ToListAsync();

                 _logger.LogInformation("Found {Count} reporting resources for userId: {UserId}", reportingIds.Count, userId);

                var result = await (from resource in _dbcontext.Resources
                                    join userKras in _dbcontext.UserKRA on resource.ResourceId equals userKras.UserId
                                    where userKras.FinalRating != null
                                          && resource.IsActive == (int)Status.IS_ACTIVE
                                          && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID
                                          && userKras.QuarterId == quarterId
                                          && userKras.IsActive == (int)Status.IS_ACTIVE
                                          && reportingIds.Contains(resource.ReportingTo ?? 0)
                                    group new { resource, userKras } by new
                                    {
                                        resource.ResourceId,
                                        resource.ResourceName
                                    } into g
                                    select new ApprovalResources
                                    {
                                        ResourceID = g.Key.ResourceId,
                                        ResourceName = g.Key.ResourceName,
                                        QuarterId = quarterId,
                                        userId = userId,
                                        approvedBy = g.Select(x => x.userKras.ApprovedBy).FirstOrDefault(),
                                        updateBy = g.Select(x => x.userKras.UpdateBy).FirstOrDefault(),
                                        IsApproved = g.Select(x => x.userKras.IsApproved).FirstOrDefault()
                                    }).ToListAsync();
                _logger.LogInformation("Retrieved {Count} pending KRAs for approval for userId: {UserId}, quarterId: {QuarterId}", result.Count, userId, quarterId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;               
            }
        }

        // Approve the resources whoes kras final rating is given.
        public async Task<bool> ResourceUpdateKraApproval(List<ResourceKraApprovalUpdate> resourceKraApprovalUpdate)
        {
            if (resourceKraApprovalUpdate == null || !resourceKraApprovalUpdate.Any())
                return false;

            foreach (var item in resourceKraApprovalUpdate)
            {
                var userKras = await _dbcontext.UserKRA
                    .Where(kra => kra.UserId == item.ResourceId
                                  && kra.QuarterId == item.QuarterId
                                  && kra.IsActive == (int)Status.IS_ACTIVE)
                    .ToListAsync();

                foreach (var kra in userKras)
                {
                    kra.ApprovedBy = item.ApprovedBy;
                    kra.UpdateBy = item.UpdatedBy;
                    kra.IsApproved = (byte?)(item.IsApproved ? 1 : 0);
                }
            }

            await _dbcontext.SaveChangesAsync();
            return true;
        }




        //Get the dates of the current quarter
        public async Task<QuarterPeriod> GetCurrentQuarterDates()
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(QuarterDetails), nameof(GetCurrentQuarterDates));
            try
            {
                var currentDate = DateTime.Now;

                var quarter = await _dbcontext.QuarterDetails
                    .FirstOrDefaultAsync(q =>
                        q.QuarterYear == currentDate.Year
                        && q.IsActive == 1
                        && q.IsDeleted == 0
                        && (
                            (q.QuarterName == "Jan-Mar" && currentDate.Month >= 1 && currentDate.Month <= 3) ||
                            (q.QuarterName == "Apr-Jun" && currentDate.Month >= 4 && currentDate.Month <= 6) ||
                            (q.QuarterName == "Jul-Sep" && currentDate.Month >= 7 && currentDate.Month <= 9) ||
                            (q.QuarterName == "Oct-Dec" && currentDate.Month >= 10 && currentDate.Month <= 12)
                        ));

                if (quarter == null)
                    return null;

                // Calculate start and end dates
                DateTime startDate, endDate;
                switch (quarter.QuarterName)
                {
                    case "Jan-Mar":
                        startDate = new DateTime((int)quarter.QuarterYear, 1, 1);
                        endDate = new DateTime((int)quarter.QuarterYear, 3, 31);
                        break;
                    case "Apr-Jun":
                        startDate = new DateTime((int)quarter.QuarterYear, 4, 1);
                        endDate = new DateTime((int)quarter.QuarterYear, 6, 30);
                        break;
                    case "Jul-Sep":
                        startDate = new DateTime((int)quarter.QuarterYear, 7, 1);
                        endDate = new DateTime((int)quarter.QuarterYear, 9, 30);
                        break;
                    case "Oct-Dec":
                        startDate = new DateTime((int)quarter.QuarterYear, 10, 1);
                        endDate = new DateTime((int)quarter.QuarterYear, 12, 31);
                        break;
                    default:
                        startDate = endDate = DateTime.MinValue;
                        break;
                }

                return new QuarterPeriod
                {                    
                    QuarterName = quarter.QuarterName,                    
                    StartDate = startDate,
                    EndDate = endDate                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }
        // Retrieves a list of active client-project assignments for a specific resource within a given quarter.
        public async Task<List<ResourceProjectAssignment>> ResourceProjectAssignment(int resourceId)
        {
            try
            {
                QuarterPeriod quarter = await GetCurrentQuarterDates();
                if (quarter == null)
                    return new List<ResourceProjectAssignment>();

                DateTime quarterStart = quarter.StartDate;
                DateTime quarterEnd = quarter.EndDate;

                var assignments = await (
                    from pr in _dbcontext.ProjectResources
                    join p in _dbcontext.Projects on pr.ProjectId equals p.ProjectId
                    join c in _dbcontext.Clients on p.ClientId equals c.ClientId
                    where pr.ResourceId == resourceId
                        && pr.IsActive == 1
                        && c.IsActive == 1
                        && pr.AssignmentDate <= quarterEnd
                        && pr.EndDate >= quarterStart
                    group pr by new { p.ProjectId, p.ProjectName, c.ClientName } into g
                    select new ResourceProjectAssignment
                    {
                        ClientName = g.Key.ClientName,
                        ProjectName = g.Key.ProjectName,
                        AssignmentDate = g.Max(x => x.StartDate),
                        ProjectEndDate = g.Max(x => x.EndDate),
                        quarterEndDate = quarterEnd
                    }
                )
                .OrderByDescending(x => x.ProjectEndDate)
                .ThenBy(x => x.ClientName)
                .ToListAsync();

                return assignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }

        // API's for getting resource list for current quarter who has not giving their rating.
        public async Task<ResourceEvaluationResponseEmails> GetResourceListOfPendingSelfEvaluations()
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ResourceEvaluationResponseEmails), nameof(GetPendingSelfEvaluations));
            try
            {
                var currentQuarter = await GetCurrentQuarter();
                if (currentQuarter == null)
                {
                    // Handle case where current quarter is not found
                    return new ResourceEvaluationResponseEmails
                    {
                        ResourceNotFilledRating = new List<ResourceEvaluationEmails>()
                    };
                }

                // Fetch raw matching data
                var rawData = await (
                    from resource in _dbcontext.Resources
                    join designatedRole in _dbcontext.DesignatedRoles
                        on resource.DesignatedRoleId equals designatedRole.DesignatedRoleId
                    join userKras in _dbcontext.UserKRA
                        on resource.ResourceId equals userKras.UserId
                    join quarters in _dbcontext.QuarterDetails
                        on userKras.QuarterId equals quarters.Id
                    where resource.IsActive == (int)Status.IS_ACTIVE
                      && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID
                      && userKras.FinalRating == null
                      && userKras.IsActive == (int)Status.IS_ACTIVE
                      && (userKras.DeveloperRating == null)
                      && (userKras.RejectedBy == null)
                      && userKras.IsApproved == 0
                      && userKras.QuarterId == currentQuarter.Id
                    select new
                    {
                        resource.ResourceId,
                        resource.ResourceName,
                        resource.EmailId,
                        quarters.Id,
                        quarters.QuarterName
                    }
                ).ToListAsync();

                // Group and format data in memory
                var ResourceNotFilledRating = rawData
                    .GroupBy(x => new { x.ResourceId, x.ResourceName, x.EmailId })
                    .Select(grouped => new ResourceEvaluationEmails
                    {                       
                        ResourceName = grouped.Key.ResourceName,
                        EmailId = grouped.Key.EmailId,
                      
                    })
                    .ToList();

                // Build and return the response
                return new ResourceEvaluationResponseEmails
                {
                    ResourceNotFilledRating = ResourceNotFilledRating
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));                
                return new ResourceEvaluationResponseEmails
                {
                    ResourceNotFilledRating = new List<ResourceEvaluationEmails>()
                };
            }
        }

    }
}

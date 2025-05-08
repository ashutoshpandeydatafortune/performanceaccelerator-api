using System;
using System.Linq;
using DF_PA_API.Models;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Services
{
    public class ProjectResourceService: IProjectResourceService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<ProjectResourceService> _logger;

        public ProjectResourceService(DFEvolutionDBContext dbContext, ILogger<ProjectResourceService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
        }

        public async Task<List<ProjectResource>> GetAllProjectResources()
        {
            return await _dbcontext.ProjectResources.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<List<ProjectResource>> GetAllProjectResourcesByResourceId(int? resourceId)
        {
            var projectResources = new List<ProjectResource>();

            try
            {
                projectResources = await (
                    from pr in _dbcontext.ProjectResources.Where(x => x.ResourceId == resourceId)
                    let resources = (_dbcontext.Resources.Where(r => (int?)r.ResourceId == pr.ResourceId)).ToList()
                    select new ProjectResource
                    {
                        ProjectResourceId = pr.ProjectResourceId,
                        ResourceId = pr.ResourceId,
                        ProjectId = pr.ProjectId,
                        ResourceRole = pr.ResourceRole,
                        CurrencyId = pr.CurrencyId,
                        BillingCycleId = pr.BillingCycleId,
                        Rate = pr.Rate,
                        Remark = pr.Remark,
                        Shadow = pr.Shadow,
                        Billable = pr.Billable,
                        CreateDate = pr.CreateDate,
                        UpdateDate = pr.UpdateDate,
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
            return projectResources;
        }

        public async Task<List<ProjectResource>> GetAllProjectResourcesByProjectId(int? projectId)
        {
            var projectResources = new List<ProjectResource>();

            try
            {
                projectResources = await (
                    from pr in _dbcontext.ProjectResources.Where(x => x.ProjectId == projectId)
                    select new ProjectResource
                    {
                        ProjectResourceId = pr.ProjectResourceId,
                        ProjectId = pr.ProjectId,
                        ResourceId = pr.ResourceId,
                        ResourceRole = pr.ResourceRole,
                        CurrencyId = pr.CurrencyId,
                        Billable = pr.Billable,
                        BillingCycleId = pr.BillingCycleId,
                        Rate = pr.Rate,
                        Remark = pr.Remark,
                        Shadow = pr.Shadow,
                        PercentageAllocation = pr.PercentageAllocation,
                        StartDate = pr.StartDate,
                        EndDate = pr.EndDate,
                        CreateBy = pr.CreateBy,
                        UpdateBy = pr.UpdateBy,
                        CreateDate = pr.CreateDate,
                        UpdateDate = pr.UpdateDate,
                        ProjectList = (_dbcontext.Projects.Where(r => (int?)r.ProjectId == pr.ProjectId)).ToList()
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return projectResources;
        }
    }
}

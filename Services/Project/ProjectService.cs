using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Utils;


namespace DF_EvolutionAPI.Services
{
    public class ProjectService: IProjectService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(DFEvolutionDBContext dbContext, ILogger<ProjectService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
        }

        public async Task<List<Project>> GetAllProjects()
        {
            return await _dbcontext.Projects.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<Project> GetProjectByProjectId(int? projectId)
        {
            var project = new Project();

            try
            {
                project = await _dbcontext.Projects.FindAsync(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return project;
        }

        public async Task<List<Project>> GetAllProjectsByProjectId(int? projectId)
        {
            var project = new List<Project>();

            try
            {
                project = await _dbcontext.Projects.Where(x => x.ProjectId == projectId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return project;
        }

        public async Task<List<ProjectResource>> GetAllProjectsByResourceId(int resourceId)
        {
            var projectResources = new List<ProjectResource>();

            try
            {
                projectResources = await (
                    from pr in _dbcontext.ProjectResources.Where(x => x.ResourceId == resourceId)
                    select new ProjectResource
                    {
                        ProjectResourceId = pr.ProjectResourceId,
                        ProjectId = pr.ProjectId,
                        ResourceRole = pr.ResourceRole,
                        CurrencyId = pr.CurrencyId,
                        BillingCycleId = pr.BillingCycleId,
                        Rate = pr.Rate,
                        Remark = pr.Remark,
                        Shadow = pr.Shadow,
                        Billable = pr.Billable,
                        CreateDate = pr.CreateDate,
                        UpdateDate = pr.UpdateDate
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
            return projectResources;
        }

        public async Task<Project> GetProjectByName(string projectName)
        {
            Project project;

            try
            {
                project = await _dbcontext.Projects.FindAsync(projectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
            
            return project;
        }
    }
}

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class ProjectService: IProjectService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        public ProjectService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<Project>> GetAllProjects()
        {
            return await _dbcontext.Projects.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<Project> GetProjectByProjectId(int? projectId)
        {
            var project = new Project();
            try
            {
                project = await _dbcontext.Projects.FindAsync(projectId);
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
                throw;
            }
            return project;
        }

        public async Task<List<ProjectResource>> GetAllProjectsByResourceId(int resourceId)
        {
            var projectResources = new List<ProjectResource>();
            try
            {
                projectResources = await (from pr in _dbcontext.ProjectResources.Where(x => x.ResourceId == resourceId)
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
                                              UpdateDate = pr.UpdateDate,
                                              //IsActive = pr.IsActive,
                                          }).ToListAsync();
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
                throw;
            }
            return project;
        }
    }
}

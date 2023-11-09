using DF_EvolutionAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    from r in _dbcontext.Resources.Where(x => x.EmailId == emailId)
                    select new Resource
                    {
                        ResourceId = r.ResourceId,
                        ResourceName = r.ResourceName,
                        ReportingTo = r.ReportingTo
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
                        ResourceProjectList = projectResources,
                    }).ToListAsync();

                foreach (var resource in resources)
                {
                    resource.ProjectList = await GetProjects(resource);
                    resource.ClientList = await GetClients(resource);
                    resource.BusinessUnits = await GetBusinessUnits(resource);
                }
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
    }
}

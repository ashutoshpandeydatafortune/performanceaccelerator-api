﻿using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using Newtonsoft.Json;
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
            return _dbcontext.Resources.Where(c => c.IsActive == 1).ToList();
        }

        public async Task<Resource> GetResourceByEmailId(string emailId)
        {
            Resource resource;

            try
            {
                resource = (
                    from r in _dbcontext.Resources
                    join designation in _dbcontext.Designations on r.DesignationId equals designation.DesignationId
                    where r.EmailId == emailId 
                    select new Resource
                    {
                        ResourceId = r.ResourceId,
                        ReportingTo = r.ReportingTo,
                        ResourceName = r.ResourceName,
                        ResourceFunctionId = r.ResourceFunctionId,
                        DesignationId = designation.DesignationId,
                        DesignationName = designation.DesignationName,
                    }
                ).FirstOrDefault();
            }
            catch (Exception ex)
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
                resources = (
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
                    }).ToList();

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
                var project = (from p in _dbcontext.Projects
                                     where p.ProjectId == rp.ProjectId && p.IsActive == 1
                                     select p).First();

                projectList.Add(project);
            }

            return projectList;
        }

        private async Task<List<Client>> GetClients(Resource resource)
        {
            var clientList = new List<Client>();

            foreach (var projectResource in resource.ProjectList)
            {
                var client = (from c in _dbcontext.Clients
                                    where c.ClientId == projectResource.ClientId && c.IsActive == 1
                                    select c).First();

                clientList.Add(client);
            }

            return clientList;
        }

        private async Task<List<BusinessUnit>> GetBusinessUnits(Resource resource)
        {
            var businessUnits = new List<BusinessUnit>();

            foreach (var c in resource.ClientList)
            {
                var businessUnit = (from b in _dbcontext.BusinessUnits
                                          where b.BusinessUnitId == c.BusinessUnitId && b.IsActive == 1
                                          select b).First();

                businessUnits.Add(businessUnit);
            }

            return businessUnits;
        }

        public async Task<string> GetChildResources(string userName)
        {
            var resources = (
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
                        DesignationId = resource.DesignationId
                    }
                ).ToList();

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
    }
}

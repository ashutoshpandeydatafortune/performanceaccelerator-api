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

            var resources = await _dbcontext.Resources.Where(c => c.IsActive == 1).ToListAsync();
            return resources;
        }

        public Task<Resource> GetResourceByEmailId(string emailId)
        {
            //var resources = await _dbcontext.Resources.Where(c => c.EmailId == emailId).ToListAsync();
            //return resources;
            //Resource resource;
            //try
            //{
            //    resource = await _dbcontext.FindAsync<Resource>(emailId);
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //return resource;


            Resource resource;
            try
            {
                resource = (from r in _dbcontext.Resources.Where(x => x.EmailId == emailId)
                                      select new Resource
                                      {
                                          ResourceId = r.ResourceId,
                                          ResourceName = r.ResourceName,
                                          ReportingTo=r.ReportingTo
                                      }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Task.FromResult(resource);
        }

        public async Task<List<Resource>> GetAllResourceDetailsByResourceId(int? resourceId)
        {
            var resourceDetails = new List<Resource>();
            try
            {
                resourceDetails = await (from r in _dbcontext.Resources.Where(x => x.ResourceId == resourceId && x.IsActive == 1)
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
                                             TenureInYears  = r.TenureInYears,
                                             TotalYears = r.TotalYears,
                                             YearBucket = r.YearBucket,
                                             CreateBy = r.CreateBy,
                                             UpdateBy = r.UpdateBy,
                                             CreateDate = r.CreateDate,
                                             UpdateDate = r.UpdateDate,
                                             ResourceProjectList = projectResources,

                                         }).ToListAsync();

                // Get Project details from Project table from ProjectResource table ProjectId

                //foreach(var rd in resourceDetails)
                //{
                //    rd.Designations = new List<Designation>();
                //    var designation = await (from d in _dbcontext.Designation
                //                             where d.DesignationId == rd.DesignationId && d.IsActive == 1
                //                             select d).FirstAsync();
                //    rd.Designations.Add(designation);
                //}
               foreach (var rd in resourceDetails)
                {
                    rd.ProjectList = new List<Project>();
                    foreach (var rp in rd.ResourceProjectList)
                    {
                        var project = await (from p in _dbcontext.Projects
                                             where p.ProjectId == rp.ProjectId && p.IsActive == 1
                                            select p).FirstAsync();
                        //_projectService.GetProjectByProjectId(rp.ProjectId);
                        //_dbcontext.Projects.FindAsync(rp.ProjectId);
                        rd.ProjectList.Add(project);
                    }
                }

                // Get Client details from Client table from Project table ClientId
                foreach (var rd in resourceDetails)
                {
                    rd.ClientList = new List<Client>();
                    foreach (var p in rd.ProjectList)
                    {
                        var client = await (from c in _dbcontext.Clients
                                            where c.ClientId == p.ClientId && c.IsActive == 1
                                            select c).FirstAsync();
                        //_clientService.GetClientByClientId(p.ClientId);
                        //_dbcontext.Clients.FindAsync(p.ClientId);
                        rd.ClientList.Add(client);
                    }
                }

                // Get Business Unit details from Business Unit table from Client table BusinessUnitId
                foreach (var rd in resourceDetails)
                {
                    rd.BusinessUnits = new List<BusinessUnit>();
                    foreach (var c in rd.ClientList)
                    {
                        var businessUnit = await (from b in _dbcontext.BusinessUnits
                                                  where b.BusinessUnitId == c.BusinessUnitId && b.IsActive == 1
                                                  select b).FirstAsync();
                        //_businessUnitService.GetBusinessUnitById(c.BusinessUnitId);
                        // _dbcontext.BusinessUnits.FindAsync(c.BusinessUnitId);
                        rd.BusinessUnits.Add(businessUnit);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return resourceDetails;
        }
    }
}

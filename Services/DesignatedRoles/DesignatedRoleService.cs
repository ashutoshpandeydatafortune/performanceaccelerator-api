using DF_EvolutionAPI;
using DF_PA_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Utils;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Services.Submission;
using NuGet.LibraryModel;
using System.Resources;

namespace DF_PA_API.Services.DesignatedRoles
{
    public class DesignatedRoleService : IDesignatedRoleService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<DesignatedRoleService> _logger;

        public DesignatedRoleService(DFEvolutionDBContext dbContext, ILogger<DesignatedRoleService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
        }

        public async Task<List<DesignatedRole>> GetAllDesignatedRoles()
        {
            try
            {
                return await (
                    from pr in _dbcontext.DesignatedRoles.Where(x => x.IsActive == (int)Status.IS_ACTIVE)
                    select new DesignatedRole
                    {
                        DesignatedRoleId = pr.DesignatedRoleId,
                        DesignatedRoleName = pr.DesignatedRoleName,
                        Description = pr.Description,
                        IsActive = pr.IsActive,
                        CreateBy = pr.CreateBy,
                        UpdateBy = pr.UpdateBy,
                        CreateDate = pr.CreateDate,
                        UpdateDate = pr.UpdateDate,
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }

        //Displayed all designatedRole by function id.
        public async Task<List<DesignatedRole>> GetDesignatedRoleByFunctionId(int functionId)
        {
            {
                try
                {
                    return await (from designatedRole in _dbcontext.DesignatedRoles
                                  join resource in _dbcontext.Resources
                                      on designatedRole.DesignatedRoleId equals resource.DesignatedRoleId
                                  where resource.FunctionId == functionId
                                        && resource.IsActive == (int)Status.IS_ACTIVE
                                        // Exclude those Designations that are already assigned to an active template
                                        && !_dbcontext.PA_TemplateDesignations
                                            .Any(templateDesignation => templateDesignation.DesignatedRoleId == designatedRole.DesignatedRoleId && templateDesignation.IsActive == 1
                                             // Check if there exists any active template (PA_Templates)
                                             && _dbcontext.PATemplates.Any(template => template.TemplateId == templateDesignation.TemplateId && template.FunctionId == functionId && template.IsActive == 1))
                                  // Group by the relevant columns from the Designation table
                                  group new { designatedRole, resource } by new
                                  {
                                      designatedRole.DesignatedRoleId,
                                      designatedRole.DesignatedRoleName,
                                      designatedRole.Description,
                                      designatedRole.IsActive,
                                      designatedRole.CreateBy,
                                      designatedRole.UpdateBy,
                                      designatedRole.CreateDate,
                                      designatedRole.UpdateDate
                                  } into grouped
                                  // Order the grouped data by DesignationName in ascending order
                                  orderby grouped.Key.DesignatedRoleName ascending
                                  // Project the grouped data into a new Designation object
                                  select new DesignatedRole
                                  {
                                      DesignatedRoleId = grouped.Key.DesignatedRoleId,
                                      DesignatedRoleName = grouped.Key.DesignatedRoleName,
                                      Description = grouped.Key.Description,
                                      IsActive = grouped.Key.IsActive,
                                      CreateBy = grouped.Key.CreateBy,
                                      UpdateBy = grouped.Key.UpdateBy,
                                      CreateDate = grouped.Key.CreateDate,
                                      UpdateDate = grouped.Key.UpdateDate

                                  }).ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                    throw;
                }
            }
        }

        public async Task<List<DesignatedRole>> GetReportingDesignatedRoles(string userName)
        {
            List<DesignatedRole> designations = new List<DesignatedRole>();

            try
            {
                designations = await (
                    from designation in _dbcontext.DesignatedRoles
                    join resource in _dbcontext.Resources on designation.DesignatedRoleId equals resource.DesignatedRoleId
                    join reportingto in _dbcontext.Resources on resource.ReportingTo equals reportingto.ResourceId
                    where reportingto.EmailId.Equals(userName) && resource.IsActive == (int)Status.IS_ACTIVE && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID && !resource.EmployeeId.StartsWith(Constant.EMPLOYEE_PREFIX)
                    select new DesignatedRole
                    {
                        DesignatedRoleId = designation.DesignatedRoleId,
                        DesignatedRoleName = designation.DesignatedRoleName
                    }).Distinct().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return designations;
        }

        public async Task<List<Resource>> GetResourcesByDesignatedRoleReporter(string designationName, int resourceId)
        {
            // We need to return all employees with passed designation who are reporting to resourceId
            List<Resource> resources = new List<Resource>();

            try
            {
                var designation = await (
                    from pr in _dbcontext.DesignatedRoles.Where(x => x.DesignatedRoleName == designationName)
                    select new Designation
                    {
                        DesignationId = pr.DesignatedRoleId,
                        DesignationName = pr.DesignatedRoleName
                    }).FirstOrDefaultAsync();

                if (designation != null)
                {
                    resources = await _dbcontext.Resources.Where(a => a.DesignatedRoleId == designation.DesignationId && a.ReportingTo == resourceId &&
                                a.IsActive == (int)Status.IS_ACTIVE && a.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID && !a.EmployeeId.StartsWith(Constant.EMPLOYEE_PREFIX))
                                .Select(x => new Resource
                                {
                                    ResourceId = x.ResourceId,
                                    ResourceName = x.ResourceName,
                                    EmailId = x.EmailId,
                                    DesignationId = x.DesignationId
                                }).ToListAsync();

                    if (resources.Count > 0)
                    {
                        foreach (var resource in resources)
                        {
                            resource.SpecialKRAs = GetAssignedSpecialKRAs(resource.ResourceId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return resources;
        }

        //Fetch Resource according to the Reportee to
        public async Task<List<ResourceReportee>> GetReporteesByManagerId(int resourceId)
        {
            List<ResourceReportee> resources = new List<ResourceReportee>();

            try
            {
                // Join Resources with DesignatedRoles and filter based on the given manager ID
                resources = await (
                    from resource in _dbcontext.Resources
                    join designation in _dbcontext.DesignatedRoles
                        on resource.DesignatedRoleId equals designation.DesignatedRoleId
                    where resource.ReportingTo == resourceId
                        && resource.IsActive == (int)Status.IS_ACTIVE
                        && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID
                        && !resource.EmployeeId.StartsWith(Constant.EMPLOYEE_PREFIX)
                    select new ResourceReportee
                    {
                        ResourceId = resource.ResourceId,
                        ResourceName = resource.ResourceName,
                        DesignationId = resource.DesignatedRoleId,
                        DesignationName = designation.DesignatedRoleName
                    }
                ).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return resources;
        }

        //Fetch Resource according to the designationated role
        public async Task<List<ResourceReportee>> GetReporteesByDesignationRole(int resourceId, string designationName)
        {
            List<ResourceReportee> resources = new List<ResourceReportee>();

            try
            {
                // Base query for reportees under the given manager ID
                var query = from resource in _dbcontext.Resources
                            join designation in _dbcontext.DesignatedRoles
                                on resource.DesignatedRoleId equals designation.DesignatedRoleId
                            where resource.ReportingTo == resourceId
                                && resource.IsActive == (int)Status.IS_ACTIVE
                                && resource.StatusId == (int)Status.ACTIVE_RESOURCE_STATUS_ID
                                && !resource.EmployeeId.StartsWith(Constant.EMPLOYEE_PREFIX)
                            select new ResourceReportee
                            {
                                ResourceId = resource.ResourceId,
                                ResourceName = resource.ResourceName,
                                DesignationId = resource.DesignatedRoleId,
                                DesignationName = designation.DesignatedRoleName
                            };

                // Apply optional filter on DesignationName if provided
                if (!string.IsNullOrEmpty(designationName))
                {
                    query = query.Where(r => r.DesignationName == designationName);
                }

                resources = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return resources;
        }

        // GetAssignedSpecialKRAs is made for displaying the special kra for particular resource.
        public List<AssignedSpecialKRA> GetAssignedSpecialKRAs(int resourceId)
        {

            try
            {
                var specialKra = (from userKras in _dbcontext.UserKRA
                                  join kraLibrary in _dbcontext.KRALibrary on userKras.KRAId equals kraLibrary.Id
                                  where kraLibrary.IsSpecial == 1 && userKras.UserId == resourceId
                                  select new AssignedSpecialKRA
                                  {
                                      KRAId = kraLibrary.Id,
                                      KraName = kraLibrary.Name,
                                  });

                return specialKra.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

        }

        // Get AssignedKRAs of resource.
        public List<AssignedSpecialKRA> GetAssignedKRAs(int resourceId)
        {
            try
            {
                var specialKra = (from userKras in _dbcontext.UserKRA
                                  join kraLibrary in _dbcontext.KRALibrary on userKras.KRAId equals kraLibrary.Id
                                  where userKras.UserId == resourceId
                                  select new AssignedSpecialKRA
                                  {
                                      KRAId = kraLibrary.Id,
                                      KraName = kraLibrary.Name,
                                  });

                return specialKra.ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }

    }
}

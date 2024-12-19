using DF_EvolutionAPI;
using DF_PA_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DF_PA_API.Services.DesignatedRoles
{
    public class DesignatedRoleService : IDesignatedRoleService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public DesignatedRoleService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
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
            catch (Exception)
            {
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
                catch (Exception)
                {
                    throw;
                }
            }
        }
        
    }
}

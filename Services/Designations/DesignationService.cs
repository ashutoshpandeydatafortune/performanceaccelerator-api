using DF_EvolutionAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DF_EvolutionAPI.Services.Designations
{
    public class DesignationService : IDesignationService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public DesignationService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<Designation> GetDesignationById(int designationId)
        {
            try
            {
                return await (
                    from pr in _dbcontext.Designations.Where(x => x.DesignationId == designationId)
                    select new Designation
                    {
                        DesignationId = pr.DesignationId,
                        DesignationName = pr.DesignationName
                    }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Designation> GetDesignationDetailsByDesignationName(string designation)
        {
            Designation designationDetails;

            try
            {
                designationDetails = await (
                    from pr in _dbcontext.Designations.Where(x => x.DesignationName == designation)
                    select new Designation
                    {
                        DesignationId = pr.DesignationId,
                        DesignationName = pr.DesignationName
                    }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return designationDetails;
        }

        public async Task<List<Resource>> GetResourcesByDesignationName(string designationName)
        {
            List<Resource> resources = new List<Resource>();

            try
            {
                var designation = await (
                    from pr in _dbcontext.Designations.Where(x => x.DesignationName == designationName)
                    select new Designation
                    {
                        DesignationId = pr.DesignationId,
                        DesignationName = pr.DesignationName
                    }).FirstOrDefaultAsync();

                if (designation != null)
                {
                    resources = await _dbcontext.Resources.Where(a => a.DesignationId == designation.DesignationId)
                                .Select(x => new Resource
                                {
                                    ResourceId = x.ResourceId,
                                    ResourceName = x.ResourceName,
                                    //ResourceProjectList = x.ResourceProjectList,
                                    EmailId = x.EmailId,
                                    DesignationId = x.DesignationId
                                }).ToListAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return resources;
        }

        public async Task<List<Resource>> GetResourcesByDesignationReporter(string designationName, int resourceId)
        {
            // We need to return all employees with passed designation who are reporting to resourceId
            List<Resource> resources = new List<Resource>();

            try
            {
                var designation = await (
                    from pr in _dbcontext.Designations.Where(x => x.DesignationName == designationName)
                    select new Designation
                    {
                        DesignationId = pr.DesignationId,
                        DesignationName = pr.DesignationName
                    }).FirstOrDefaultAsync();

                if (designation != null)
                {
                    resources = await _dbcontext.Resources.Where(a => a.DesignationId == designation.DesignationId && a.ReportingTo == resourceId)
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
            catch (Exception)
            {
                throw;
            }

            return resources;
        }

        // GetAssignedSpecialKRAs is made for displaying the special kra for particular resource.
        private List<AssignedSpecialKRA> GetAssignedSpecialKRAs(int resourceId)
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

        public async Task<List<Designation>> GetReportingDesignations(string userName)
        {
            List<Designation> designations = new List<Designation>();

            try
            {
                designations = await (
                    from designation in _dbcontext.Designations
                    join resource in _dbcontext.Resources on designation.DesignationId equals resource.DesignationId
                    join reportingto in _dbcontext.Resources on resource.ReportingTo equals reportingto.ResourceId
                    where reportingto.EmailId.Equals(userName)
                    select new Designation
                    {
                        DesignationId = designation.DesignationId,
                        DesignationName = designation.DesignationName
                    }).Distinct().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return designations;
        }

        public async Task<List<Designation>> GetAllDesignations()
        {
            {
                try
                {
                    return await (
                        from pr in _dbcontext.Designations.Where(x => x.IsActive == 1)
                        select new Designation
                        {
                            DesignationId = pr.DesignationId,
                            DesignationName = pr.DesignationName,
                            ReferenceId = pr.ReferenceId,
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
        }
        //Displayed all designation by function id.
        public async Task<List<Designation>> GetDesignationByFunctionId(int resourceFunctionId)
        {
            {
                try
                {
                    return await (  from designation in _dbcontext.Designations
                                    join resource in _dbcontext.Resources on designation.DesignationId equals resource.DesignationId
                                    where resource.FunctionId == resourceFunctionId && resource.IsActive == 1
                                    group new { designation, resource } by new { designation.DesignationId, designation.ReferenceId, designation.DesignationName, designation.IsActive, designation.CreateBy, designation.UpdateBy, designation.CreateDate, designation.UpdateDate } into grouped
                                    select new Designation
                                    {
                                        DesignationId = grouped.Key.DesignationId,
                                        ReferenceId = grouped.Key.ReferenceId,
                                        DesignationName = grouped.Key.DesignationName,
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
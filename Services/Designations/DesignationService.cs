using DF_EvolutionAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Designations
{
    public class DesignationService : IDesignationService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        
        public DesignationService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<Designation> GetDesignationDetailsByDesignationName(string designation)
        {
            Designation designationDetails;

            try
            {
                designationDetails = await (
                    from pr in _dbcontext.Designation.Where(x => x.DesignationName == designation)
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
                var designation = (
                    from pr in _dbcontext.Designation.Where(x => x.DesignationName == designationName)
                    select new Designation
                    {
                         DesignationId = pr.DesignationId,
                         DesignationName = pr.DesignationName
                    }).FirstOrDefault();

                if (designation != null)
                {
                    resources = _dbcontext.Resources.Where(a => a.DesignationId == designation.DesignationId)
                                .Select(x => new Resource
                                {
                                    ResourceId = x.ResourceId,
                                    ResourceName = x.ResourceName,
                                    //ResourceProjectList = x.ResourceProjectList,
                                    EmailId = x.EmailId,
                                    DesignationId = x.DesignationId
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return  resources;
        }
        public async Task<List<Designation>> GetReportingDesignations(string userName)
        {
            List<Designation> designations = new List<Designation>();

            try
            {
                designations = (
                    from designation in _dbcontext.Designation 
                    join resource in _dbcontext.Resources on designation.DesignationId equals resource.DesignationId
                    join reportingto in _dbcontext.Resources on resource.ReportingTo equals reportingto.ResourceId 
                    where reportingto.EmailId.Equals(userName)
                    select new Designation
                    {
                        DesignationId = designation.DesignationId,
                        DesignationName = designation.DesignationName
                    }).Distinct().ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return designations;
        }
    }
}

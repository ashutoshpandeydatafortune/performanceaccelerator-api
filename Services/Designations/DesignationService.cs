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

        public async Task<List<Resource>> GetResourcesByDesignationName(string designation)
        {
            List<Resource> resources = new List<Resource>();

            try
            {
                var designationDetail = (
                    from pr in _dbcontext.Designation.Where(x => x.DesignationName == designation)
                    select new Designation
                    {
                         DesignationId = pr.DesignationId,
                         DesignationName = pr.DesignationName
                    }).FirstOrDefault();

                if (designationDetail != null)
                {
                    resources = _dbcontext.Resources.Where(a => a.DesignationId == designationDetail.DesignationId)
                                .Select(x => new Resource
                                {
                                    ResourceId = x.ResourceId,
                                    ResourceName = x.ResourceName,
                                    ResourceProjectList = x.ResourceProjectList,
                                    EmailId = x.EmailId,
                                    DesignationId = x.DesignationId
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return  resources;
        }
    }
}

using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Designations
{
    public interface IDesignationService
    {
        public Task<Designation> GetDesignationById(int designationId);
        public Task<List<Designation>> GetReportingDesignations(string userName);
        public Task<List<Resource>> GetResourcesByDesignationName(string designation);
        public Task<Designation> GetDesignationDetailsByDesignationName(string designation);
        public Task<List<Resource>> GetResourcesByDesignationReporter(string designation, int resourceId);
        public Task<List<Designation>> GetAllDesignations();
        public Task<List<Designation>> GetDesignationByFunctionId(int resourceFunctionId);
    }
}

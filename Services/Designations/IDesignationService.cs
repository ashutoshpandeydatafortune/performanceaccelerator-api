using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Designations
{
    public interface IDesignationService
    {
        public Task<List<Resource>> GetResourcesByDesignationName(string designation);
        public Task<Designation> GetDesignationDetailsByDesignationName(string designation);
    }
}

using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DF_EvolutionAPI.Services.Designations
{
    public interface IDesignationService
    {
        public Task<Designation> GetDesignationDetailsByDesignationName(string designation);
        public List<Resource> GetResourcesByDesignationName(string designation);
    }
}

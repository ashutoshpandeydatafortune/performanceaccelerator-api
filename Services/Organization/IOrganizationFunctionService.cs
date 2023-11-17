using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IOrganizationFunctionService
    {
        public Task<List<OrganizationFunction>> GetAllFunctions();
        public Task<OrganizationFunction> GetFunctionById(int functionId);
    }
}

using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IResourceFunctionService
    {
        public Task<List<ResourceFunction>> GetAllFunctions();
        public Task<ResourceFunction> GetFunctionById(int functionId);
    }
}

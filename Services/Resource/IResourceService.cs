using System.Collections.Generic;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;

namespace DF_EvolutionAPI.Services
{
    public interface IResourceService
    {
        public Task<List<Resource>> GetAllResources();
        public Task<Resource> GetResourceByEmailId(string EmailId);
        public Task<List<Resource>> GetAllResourceDetailsByResourceId(int? resourceId);
    }
}

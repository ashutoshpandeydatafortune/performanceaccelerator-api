using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IProjectResourceService
    {
        public Task<List<ProjectResource>> GetAllProjectResources();
        public Task<List<ProjectResource>> GetAllProjectResourcesByProjectId(int? projectId);
        public Task<List<ProjectResource>> GetAllProjectResourcesByResourceId(int? resourceId);
    }
}

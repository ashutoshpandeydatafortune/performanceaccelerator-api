using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IProjectService
    {
        public Task<List<Project>> GetAllProjects();
        public Task<Project> GetProjectByName(string name);
        public Task<Project> GetProjectByProjectId(int? projectId);
        public Task<List<Project>> GetAllProjectsByProjectId(int? projectId);
        public Task<List<ProjectResource>> GetAllProjectsByResourceId(int resourceId);
    }
}

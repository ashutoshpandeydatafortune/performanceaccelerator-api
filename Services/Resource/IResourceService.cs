using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IResourceService
    {
        Task<List<Resource>> GetAllResources();
        Task<string> GetMyTeamDetails(int userId);
        Task<string> GetChildResources(string userName);
        Task<Resource> GetProfileDetails(int? resourceId);
        Task<Resource> GetResourceByEmailId(string EmailId);
        Task<List<Resource>> GetAllResourceDetailsByResourceId(int? resourceId);


    }
}

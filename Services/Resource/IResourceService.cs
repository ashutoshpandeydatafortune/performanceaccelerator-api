using System.Collections.Generic;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;

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

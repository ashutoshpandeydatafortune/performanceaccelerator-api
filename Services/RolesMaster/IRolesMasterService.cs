using DF_EvolutionAPI.ViewModels;
using DF_PA_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_PA_API.Services.RolesMaster
{
    public interface IRolesMasterService
    {
        Task<ResponseModel> CreateRoleMaster(RoleMaster roleMaster);
        Task<ResponseModel> UpdateRoleMaster(RoleMaster roleMaster);
        Task<List<RoleMaster>> GetAllRolesMaster();
        Task<ResponseModel> DeleteRoleMasterById(string id);

    }
}

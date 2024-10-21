using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Services
{
    public interface ISettingsService
    {
        public Task<List<RoleMapping>> GetPermissionsByRole( string roleId);
        public Task<ResponseModel> UpdatePermissionByRole(RoleMapping roleMapping);
        public Task<ResponseModel> AddRoleMapping(List<RoleMapping> roleMappings);        
        public Task<List<UserResourceRole>> GetAllUsersRole();
        public Task<List<RoleMaster>> GetAllRoles();
        public Task<userEmail> GetAdminEmail();
        public ResponseModel CreateOrUpdateUserRole(UserRoles userRoles);
    }
}

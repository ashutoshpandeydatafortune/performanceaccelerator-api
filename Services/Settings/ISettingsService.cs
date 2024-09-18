using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using DF_PA_API.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface ISettingsService
    {
        public Task<List<RoleMapping>> GetPermissionsByRole( string roleId);
        public Task<ResponseModel> UpdatePermissionByRole(RoleMapping roleMapping);
        public Task<ResponseModel> AddRoleMapping(List<RoleMapping> roleMappings);
        //public Task<List<IdentityRole>> GetAllRoles();
        public Task<List<RoleMaster>> GetAllRoles();
        public ResponseModel CreateOrUpdateUserRole(string emailId, string roleName);
    }
}

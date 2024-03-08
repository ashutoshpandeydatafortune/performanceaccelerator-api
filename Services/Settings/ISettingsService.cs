using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface ISettingsService
    {
        public Task<List<RoleMapping>> GetPermissionByRole( string roleId);
        public Task<ResponseModel> UpdatePermissionByRole(RoleMapping roleMapping);
        public Task<List<IdentityRole>> GetAllRoles();
    }
}

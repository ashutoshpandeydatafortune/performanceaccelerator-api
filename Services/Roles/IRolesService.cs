using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IRolesService
    {
        public Task<List<Roles>> GetAllRoleList();

        public Task<Roles> GetRoleById(int roleId);

        public Task<ResponseModel> CreateorUpdateRole(Roles rolesModel);

        public Task<ResponseModel> DeleteRole(int roleId);
    }
}

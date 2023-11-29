using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Services
{
    public interface IRolesService
    {
        public List<Role> GetAllRoleList();
        public Role GetRoleById(int roleId);
        public ResponseModel DeleteRole(int roleId);
        public ResponseModel CreateorUpdateRole(Role rolesModel);
    }
}

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IRoleMappingService 
    {
        public Task<List<RoleMapping>> GetAllRoleMappingList();

        public Task<RoleMapping> GetRoleMappingById(int roleMappingId);

        public Task<ResponseModel> CreateorUpdateRoleMapping(RoleMapping roleMappingModel);

        public Task<ResponseModel> DeleteRoleMapping(int roleMappingId);

        public Task<List<RoleMapping>> GetRoleMappingByRoleId(int roleId);

        public Task<List<RoleMapping>> GetRoleMappingByUserId(int userId);

        //public Task<ResponseModel> GetRoleMappingByEmail(int roleMappingId);
    }
}

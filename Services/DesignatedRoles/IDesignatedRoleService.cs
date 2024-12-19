using DF_EvolutionAPI.Models;
using DF_PA_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_PA_API.Services.DesignatedRoles
{
    public interface IDesignatedRoleService
    {
        public Task<List<DesignatedRole>> GetAllDesignatedRoles();
        public Task<List<DesignatedRole>> GetDesignatedRoleByFunctionId(int functionId);
       
    }
}

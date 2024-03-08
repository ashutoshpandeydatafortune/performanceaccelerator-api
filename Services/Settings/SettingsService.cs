using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DF_EvolutionAPI.Services
{
    public class SettingsService:ISettingsService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        public SettingsService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }
        //Displays all the assign roles for particular roleid
        public async Task<List<RoleMapping>> GetPermissionByRole(string roleId)
        {
            return  _dbcontext.PA_RoleMappings
                .Where(r => r.RoleId == roleId)
                .ToList();           
        }
        
        //public async Task<List<IdentityRole>>GetAllRoles()
        //{
        //    return  _dbcontext.AspNetRoles.ToList();
        //}

    }
}

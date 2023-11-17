using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Linq;
using System;

namespace DF_EvolutionAPI.Services
{
    public class OrganizationFunctionService : IOrganizationFunctionService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public OrganizationFunctionService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<OrganizationFunction>> GetAllFunctions()
        {
            return await _dbcontext.OrganizationFunctions.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<OrganizationFunction> GetFunctionById(int functionId)
        {
            OrganizationFunction organizationFunction;

            try
            {
                organizationFunction = await _dbcontext.FindAsync<OrganizationFunction>(functionId);
            }
            catch (Exception)
            {
                throw;
            }
            
            return organizationFunction;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Linq;
using System;

namespace DF_EvolutionAPI.Services
{
    public class ResourceFunctionService : IResourceFunctionService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public ResourceFunctionService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<ResourceFunction>> GetAllFunctions()
        {
            return await _dbcontext.ResourceFunctions.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<ResourceFunction> GetFunctionById(int functionId)
        {
            ResourceFunction resourceFunction;

            try
            {
                resourceFunction = await _dbcontext.FindAsync<ResourceFunction>(functionId);
            }
            catch (Exception)
            {
                throw;
            }
            
            return resourceFunction;
        }
    }
}

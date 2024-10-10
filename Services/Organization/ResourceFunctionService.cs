using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DF_EvolutionAPI.Services

{
    public class ResourceFunctionService : IResourceFunctionService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public ResourceFunctionService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<TechFunction>> GetAllFunctions()
        {
            return await _dbcontext.TechFunctions.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<TechFunction> GetFunctionById(int functionId)
        {
            TechFunction resourceFunction;

            try
            {
                resourceFunction = await _dbcontext.FindAsync<TechFunction>(functionId);
            }
            catch (Exception)
            {
                throw;
            }
            
            return resourceFunction;
        }
    }
}

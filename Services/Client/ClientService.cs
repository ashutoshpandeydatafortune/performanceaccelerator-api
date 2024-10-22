using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DF_EvolutionAPI.Services
{
    public class ClientService: IClientService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public ClientService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<Client>> GetAllClients()
        {
            return await _dbcontext.Clients.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<Client> GetClientByClientId(int clientId)
        {
            Client client;

            try
            {
                client = await _dbcontext.FindAsync<Client>(clientId);
            }
            catch (Exception)
            {
                throw;
            }
            
            return client;
        }
    }
}

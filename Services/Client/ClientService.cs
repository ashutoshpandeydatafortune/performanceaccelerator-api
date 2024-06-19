using DF_EvolutionAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class ClientService : IClientService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public ClientService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<Client>> GetAllClients()
        {
            return await _dbcontext.Clients.Where(c => c.IsActive == 1).ToListAsync();
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

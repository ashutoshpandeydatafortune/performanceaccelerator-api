using System;
using System.Linq;
using DF_PA_API.Models;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace DF_EvolutionAPI.Services
{
    public class ClientService: IClientService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<ClientService> _logger;

        public ClientService(DFEvolutionDBContext dbContext, ILogger<ClientService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
            
            return client;
        }
    }
}

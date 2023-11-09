using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IClientService
    {
        public Task<List<Client>> GetAllClients();
        public Task<Client> GetClientByClientId(int clinetId);
    }
}

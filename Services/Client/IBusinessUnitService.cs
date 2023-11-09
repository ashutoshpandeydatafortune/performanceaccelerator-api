using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IBusinessUnitService
    {
        public Task<List<BusinessUnit>> GetAllBusinessUnits();
        public Task<List<Client>> GetAllClientsBusinessUnitId(int? businessUnitId);
        public Task<List<BusinessUnit>> GetAllClientsBusinessUnits(int? businessUnitId);

    }
}

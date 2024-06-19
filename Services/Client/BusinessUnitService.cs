using DF_EvolutionAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class BusinessUnitService : IBusinessUnitService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public BusinessUnitService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<BusinessUnit>> GetAllBusinessUnits()
        {
            return await _dbcontext.BusinessUnits.ToListAsync();
        }

        public async Task<List<BusinessUnit>> GetAllClientsBusinessUnits(int? businessUnitId)
        {
            var businessUnits = new List<BusinessUnit>();
            try
            {
                businessUnits = await (
                    from b in _dbcontext.BusinessUnits.Where(x => x.IsActive == 1)
                    let clients = (_dbcontext.Clients.Where(c => (int?)c.BusinessUnitId == businessUnitId && c.IsActive == 1)).ToList()
                    select new BusinessUnit
                    {
                        BusinessUnitId = b.BusinessUnitId,
                        BusinessUnitName = b.BusinessUnitName,
                        Remark = b.Remark,
                        IsActive = b.IsActive,
                        CreateBy = b.CreateBy,
                        UpdateBy = b.UpdateBy,
                        CreateDate = b.CreateDate,
                        UpdateDate = b.UpdateDate,
                        ClientsList = clients
                    }).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return businessUnits;
        }

        public async Task<List<Client>> GetAllClientsBusinessUnitId(int? businessUnitId)
        {
            var clients = new List<Client>();

            try
            {
                clients = await _dbcontext.Clients.Where(x => (x.IsActive == 1) && (x.BusinessUnitId == businessUnitId)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return clients;
        }
    }
}

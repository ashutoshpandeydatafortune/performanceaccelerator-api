using System;
using System.Linq;
using DF_PA_API.Models;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Utils;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace DF_EvolutionAPI.Services
{
    public class BusinessUnitService : IBusinessUnitService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<BusinessUnitService> _logger;

        public BusinessUnitService(DFEvolutionDBContext dbContext, ILogger<BusinessUnitService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
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
                    from b in _dbcontext.BusinessUnits.Where(x => x.IsActive == (int)Status.IS_ACTIVE)
                    let clients = (_dbcontext.Clients.Where(c => (int?)c.BusinessUnitId == businessUnitId && c.IsActive == (int)Status.IS_ACTIVE)).ToList()
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
            catch(Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return businessUnits;
        }

        public async Task<List<Client>> GetAllClientsBusinessUnitId(int? businessUnitId)
        {
            var clients = new List<Client>();

            try
            {
                clients = await _dbcontext.Clients.Where(x => (x.IsActive == (int)Status.IS_ACTIVE) && (x.BusinessUnitId == businessUnitId)).ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return clients;
        }
    }
}

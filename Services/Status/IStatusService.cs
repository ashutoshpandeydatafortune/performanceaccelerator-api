using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IStatusService
    {
        public Task<List<StatusLibrary>> GetAllStatusList();

        public Task<StatusLibrary> GetStatusById(int statusId);

        public Task<ResponseModel> CreateorUpdateStatus(StatusLibrary statusModel);

        public Task<ResponseModel> DeleteStatus(int statusId);
    }
}

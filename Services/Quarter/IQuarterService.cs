using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IQuarterService
    {
        public Task<ResponseModel> DeleteQuarter(int quarterId);
        public Task<List<QuarterDetails>> GetAllQuarterList(string type);
        public Task<QuarterDetails> GetQuarterDetailsById(int quarterId);
        public Task<ResponseModel> GetStatusDetailsByQuarterId(int quarterId);
        public Task<ResponseModel> CreateorUpdateQuarter(QuarterDetails quarterModel);
    }
}

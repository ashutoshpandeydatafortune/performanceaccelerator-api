using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Services
{
    public interface IQuarterService
    {
        public List<QuarterDetails> GetAllQuarterList();
        public ResponseModel DeleteQuarter(int quarterId);
        public QuarterDetails GetQuarterDetailsById(int quarterId);
        public ResponseModel GetStatusDetailsByQuarterId(int quarterId);
        public ResponseModel CreateorUpdateQuarter(QuarterDetails quarterModel);
    }
}

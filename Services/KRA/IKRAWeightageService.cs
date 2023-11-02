using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IKRAWeightageService
    {
        public Task<List<KRAWeightage>> GetAllKRAWeightageList();

        public Task<KRAWeightage> GetKRAWeightageDetailsById(int weightageId);

        public Task<ResponseModel> CreateorUpdateKRAWeightage(KRAWeightage weightageModel);

        public Task<ResponseModel> DeleteKRAWeightage(int weightageId);

    }
}

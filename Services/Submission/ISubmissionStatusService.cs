using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Submission
{
    public interface ISubmissionStatusService
    {
        public Task<List<SubmissionStatus>> GetAllSubmissionStatusList();
        public Task<ResponseModel> DeleteSubmissionStatus(int submissionStatusId);
        public Task<SubmissionStatus> GetSubmissionStatusById(int submissionStatusId);
        public Task<ResponseModel> CreateorUpdateSubmissionStatus(SubmissionStatus statusModel);
    }
}

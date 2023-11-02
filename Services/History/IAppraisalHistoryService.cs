using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.History
{
    public interface IAppraisalHistoryService
    {
        public Task<List<AppraisalHistory>> GetAllAppraisalHistoryList();

        public Task<AppraisalHistory> GetAppraisalHistoryById(int appraisalHistoryId);

        public Task<ResponseModel> CreateorUpdateAppraisalHistory(AppraisalHistory appraisalHistoryModel);

        public Task<ResponseModel> DeleteAppraisalHistory(int appraisalHistoryId);

        public Task<ResponseModel> GetAppraisalHistoryByUserId(int userId);

        public Task<ResponseModel> GetUserDetailsByAppraisalHistoryId(int appraisalHistoryId);
    }
}

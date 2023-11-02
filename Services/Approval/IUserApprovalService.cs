using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface IUserApprovalService
    {
        public Task<List<UserApproval>> GetAllApprovalList();

        public Task<UserApproval> GetApprovalById(int roleId);

        public Task<ResponseModel> CreateorUpdateApproval(UserApproval userApprovalModel);

        public Task<ResponseModel> DeleteApproval(int userApprovalId);
    }
}

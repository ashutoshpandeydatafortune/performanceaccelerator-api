using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DF_EvolutionAPI.Models.UserKRADetails;


namespace DF_EvolutionAPI.Services.KRA
{
    public interface IUserKRAService
    {
        public Task<List<UserKRA>> GetAllUserKRAList();
        public Task<UserKRA> GetUserKRAById(int userKRAId);
        public Task<ResponseModel> DeleteUserKRA(int userKRAId);
        public List<UserKRADetails> GetKRAsByUserId(int? UserId);
        Task<ResponseModel> UpdateUserKra(UpdateUserKRARequest request);
        public Task<ResponseModel> CreateUserKRA(List<UserKRA> userKRAModel);
        public List<UserAssignedKRA> GetAssignedKRAsByDesignation(string designation);
        public List<UserAssignedKRA> GetAssignedKRAsByDesignationId(int designationId);
        public List<UserKRARatingList> GetUserKraGraph(int userId, string quarterYearRange);
        public Task<ResponseModel> AssignUnassignKra(int userKraId, byte IsActive);
        public Task<List<UserKRA>> GetReleasedKraUsers(int quarterId, int managerId);
        public Task<List<AssignedKras>> GetResourceReleasedKras(int quarterId, int userId);


    }
}

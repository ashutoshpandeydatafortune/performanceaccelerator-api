using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.KRA
{
    public interface IUserKRAService
    {
        public Task<List<UserKRA>> GetAllUserKRAList();
        public Task<UserKRA> GetUserKRAById(int userKRAId);
        public Task<ResponseModel> DeleteUserKRA(int userKRAId);
        public List<UserKRADetails> GetKRAsByUserId(int? UserId);
        public Task<ResponseModel> CreateUserKRA(List<UserKRA> userKRAModel);
        public ResponseModel CreateorUpdateUserKRA(UserKRA userKRAModel);
        public List<UserAssignedKRA> GetAssignedKRAsByDesignation(string designation);
        Task<ResponseModel> UpdateUserKra(List<UserKRA> userKRAModels);
    }
}

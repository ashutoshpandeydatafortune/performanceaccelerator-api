using DF_EvolutionAPI.Models;
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
        public Task<ResponseModel> CreateUserKRA(UserKRA userKRAModel);
        public Task<List<UserKRADetails>> GetKRAsByUserId(int? UserId);
        public Task<ResponseModel> CreateorUpdateUserKRA(UserKRA userKRAModel);
    }
}

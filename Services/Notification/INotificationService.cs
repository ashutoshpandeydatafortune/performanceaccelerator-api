using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public interface INotificationService
    {
        public Task<ResponseModel> CreateNotification(Notification notificationModel);
        public Task<List<Notification>> GetNotificationsByResourceId(int resourceId);
        public Task<Notification> GetNotificationById(int Id);
    }
}

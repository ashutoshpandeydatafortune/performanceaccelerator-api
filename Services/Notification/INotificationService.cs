using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Services
{
    public interface INotificationService
    {
        public Task<Notification> GetNotificationsById(int Id);
        public Task<ResponseModel> CreateNotification(Notification notificationModel);      
        public Task<List<Notification>> GetNotificationsByResourceId(int resourceId);
    }
}

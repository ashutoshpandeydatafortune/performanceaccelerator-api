using System;
using System.Linq;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DF_EvolutionAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly DFEvolutionDBContext _dbContext;
        public NotificationService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel> CreateNotification(Notification notificationModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                notificationModel.IsRead = 0;
                notificationModel.IsActive = 1;
                notificationModel.CreateAt = DateTime.Now;

                _dbContext.Add(notificationModel);
                model.Messsage = "Notification Added Successfully.";

                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error :" + ex.Message;
            }
            return model;
        }

        //Get all the Active notification and update it.
        public async Task<List<Notification>> GetNotificationsByResourceId(int resourceId)
        {
            try
            {
                DateTime cutoffDate = DateTime.Now.AddDays(Constant.DAYS_TO_LOOK_BACK);

                // Get notifications that meet the criteria
                List<Notification> notifications = await GetNotifications(resourceId, cutoffDate);

                // Mark notifications as read
                await MarkNotificationsAsRead(notifications);

                return notifications;
            }
            catch (Exception ex)
            {
                // Handle the exception here, log it, or return an error response
                Console.WriteLine($"An error occurred while getting notifications: {ex.Message}");
                return new List<Notification>();
            }
        }

        //Method for fetching notifications that meets the criteria
        private async Task<List<Notification>> GetNotifications(int resourceId, DateTime cutoffDate)
        {
            return  await _dbContext.Notifications
                .Where(notification => notification.ResourceId == resourceId
                    && notification.IsActive == 1 && notification.CreateAt > cutoffDate)
                .OrderByDescending(notification => notification.CreateAt)
                .ToListAsync();
        }

        //Method for updating IsRead flag as true.
        private async Task MarkNotificationsAsRead(IEnumerable<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                notification.IsRead = 1;
                notification.UpdateAt = DateTime.Now;
            }

            _dbContext.Notifications.UpdateRange(notifications);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Notification> GetNotificationsById(int Id)
        {
            return await _dbContext.Notifications.Where(notification => notification.Id == Id && notification.IsActive == 1)
                 .FirstOrDefaultAsync();

        }        
    }
}

﻿using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DF_EvolutionAPI.Utils;
//using System.Reflection.Metadata;

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
        //Get all the Active notification and update
        public async Task<List<Notification>> GetNotificationsByResourceId(int resourceId)
        {           
                DateTime cutoffDate = DateTime.Now.AddDays(Constant.NotificationDays);

                List<Notification> notifications = _dbContext.Notifications
                    .Where(notification => notification.ResourceId == resourceId
                        && notification.IsActive == 1 && notification.CreateAt > cutoffDate)
                    .ToList();

                if (notifications != null && notifications.Count > 0)
                {
                    await MarkNotificationsAsRead(notifications);
                }

                return notifications;
                       
        }

        //Method for updating IsRead flag as true.
        public async Task MarkNotificationsAsRead(List<Notification> notifications)
        {      
                foreach (var notification in notifications)
                {
                    notification.IsRead = 1;
                    notification.UpdateAt = DateTime.Now;

                    _dbContext.Notifications.Update(notification);
                }
                await _dbContext.SaveChangesAsync();            
        }

        public async Task<Notification> GetNotificationsById(int Id)
        {
            return _dbContext.Notifications.Where(notification => notification.Id == Id && notification.IsActive == 1)
                 .FirstOrDefault();

        }

        public async Task<ResponseModel> UpdateNotifications(Notification notificationModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                Notification updateNotifications = _dbContext.Notifications.Find(notificationModel.Id);
                if (updateNotifications != null)
                {
                    updateNotifications.IsRead = 1;
                    updateNotifications.Title = notificationModel.Title;
                    updateNotifications.Description = notificationModel.Description;
                    updateNotifications.IsActive = 1;
                    updateNotifications.UpdateAt = DateTime.Now;

                    await _dbContext.SaveChangesAsync();

                    model.IsSuccess = true;
                    model.Messsage = "Notification Updated Successfully.";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Notification does not exist.";
                }
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error =" + ex.Message;
            }
            return model;
        }       
    }
}

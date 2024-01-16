using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRA;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Services.Email;
using System.Text;

namespace DF_EvolutionAPI.Services
{
    public class UserKRAService : IUserKRAService
    {
        private readonly FileUtil _fileUtil;
        private readonly IEmailService _emailService;
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserKRAService(DFEvolutionDBContext dbContext, IWebHostEnvironment hostingEnvironment, IEmailService emailService, FileUtil fileUtil)
        {
            _fileUtil = fileUtil;
            _dbcontext = dbContext;
            _emailService = emailService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<List<UserKRA>> GetAllUserKRAList()
        {
            return await _dbcontext.UserKRA.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<UserKRA> GetUserKRAById(int userKRAId)
        {
            UserKRA userKRA;
            try
            {
                userKRA = await _dbcontext.UserKRA.Where(c => c.IsActive == 1 && c.Id == userKRAId).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return userKRA;
        }

        public List<UserAssignedKRA> GetAssignedKRAsByDesignation(string designation)
        {
            try
            {
                var query = from resource in _dbcontext.Resources
                            join userKRA in _dbcontext.UserKRA on resource.ResourceId equals userKRA.UserId
                            join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                            select new UserAssignedKRA()
                            {
                                KRAName = kraLibrary.Name,
                                KRAId = userKRA.KRAId.Value,
                                UserId = resource.ResourceId,
                                UserName = resource.ResourceName,
                                IsSpecial = kraLibrary.IsSpecial.Value,
                                QuarterId = userKRA.QuarterId.HasValue ? userKRA.QuarterId.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseModel> CreateUserKRA(List<UserKRA> userKRAModel)
        {
            /**
             * This method performs following actions:
             * 1. Run loop on passed kras and store in database
             * 2. Find the user from passed kras
             * 3. Prepare and store created kras notifications in database
             * 4. Compile kras content and send single email to user
             */

            ResponseModel model = new ResponseModel();

            bool created = await CreateKraEntries(userKRAModel);
            if (created)
            {
                Dictionary<int, UserNotificationData> notificationMap = await CreateNotifications(userKRAModel);
                foreach (var entry in notificationMap)
                {
                    await SendNotification(entry.Value, "kra-created.html");
                }

                model.IsSuccess = true;
            }
            else
            {
                model.IsSuccess = false;
            }

            return model;
        }

        public async Task<bool> CreateKraEntries(List<UserKRA> userKRAModel)
        {
            try
            {
                foreach (var item in userKRAModel)
                {
                    item.ManagerComment = string.IsNullOrEmpty(item.DeveloperComment) ? "" : item.DeveloperComment;
                    item.DeveloperComment = string.IsNullOrEmpty(item.DeveloperComment) ? "" : item.DeveloperComment;
                    item.ApprovedBy = item.ApprovedBy == null ? item.ApprovedBy : item.ApprovedBy;
                    item.RejectedBy = item.RejectedBy == null ? item.RejectedBy : item.RejectedBy;

                    item.IsActive = 1;
                    item.CreateBy = 1;
                    item.UpdateBy = 1;
                    item.CreateDate = DateTime.Now;
                    item.UpdateDate = DateTime.Now;

                    await _dbcontext.AddAsync(item);
                }

                await _dbcontext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

            return true;
        }

        public async Task<Dictionary<int, UserNotificationData>> CreateNotifications(List<UserKRA> userKRAModel)
        {
            try
            {
                Dictionary<int, UserNotificationData> notificationMap = await PrepareNotifications(userKRAModel);

                foreach (var entry in notificationMap)
                {
                    await InsertNotifications(entry.Value.Notifications);
                }

                return notificationMap;
            }
            catch
            {
                throw;
            }
        }

        private async Task<Dictionary<int, UserNotificationData>> PrepareNotifications(List<UserKRA> userKRAModel)
        {
            Dictionary<int, UserNotificationData> notificationMap = new Dictionary<int, UserNotificationData>();
            
            foreach (UserKRA userKRA in userKRAModel)
            {
                if (!notificationMap.ContainsKey(userKRA.UserId.Value))
                {
                    UserNotificationData userNotificationData = new UserNotificationData();
                    userNotificationData.Notifications = new List<Notification>();

                    notificationMap[userKRA.UserId.Value] = userNotificationData;
                }

                // Find user details
                var user =  _dbcontext.Resources
                               //.Where(resource => resource.ResourceId == userKRAModel[0].UserId)
                               .Where(resource => resource.ResourceId == userKRA.UserId.Value)
                               .FirstOrDefault();
                
                 Notification notification = new Notification
                {
                    ResourceId = userKRA.UserId.Value,
                    Title = Constant.SUBJECT_KRA_CREATED,
                    Description = userKRA.KRAName,     // store kra name
                    IsRead = 0,
                    IsActive = 1,
                    CreateAt = DateTime.Now
                };

                notificationMap[userKRA.UserId.Value].Email = user.EmailId;
                notificationMap[userKRA.UserId.Value].Name = user.ResourceName;
                notificationMap[userKRA.UserId.Value].Notifications.Add(notification);
            }

            return notificationMap;
        }

        private List<int> GetUserIds(List<UserKRA> userKRAModel)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> UpdateUserKra(List<UserKRA> userKRAModels)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                foreach (var userKRAModel in userKRAModels)
                {
                    var userKra = await _dbcontext.UserKRA.FindAsync(userKRAModel.Id);

                    if (userKra != null)
                    {
                        userKra.Reason = userKRAModel.Reason;
                        userKra.Comment = userKRAModel.Comment;
                        userKra.ApprovedBy = userKRAModel.ApprovedBy;
                        userKra.RejectedBy = userKRAModel.RejectedBy;
                        userKra.FinalComment = userKRAModel.FinalComment;
                        userKra.ManagerComment = userKRAModel.ManagerComment;
                        userKra.FinalRating = userKRAModel.FinalRating ?? null;
                        userKra.DeveloperComment = userKRAModel.DeveloperComment;
                        userKra.ManagerRating = userKRAModel.ManagerRating ?? null;
                        userKra.AppraisalRange = userKRAModel.AppraisalRange ?? null;
                        userKra.DeveloperRating = userKRAModel.DeveloperRating ?? null;

                        userKra.IsActive = 1;
                        userKra.UpdateBy = 1;
                        userKra.UpdateDate = DateTime.Now;


                        await _dbcontext.SaveChangesAsync();
                    }
                    else
                    {
                        model.Messsage = "User KRA not found for update.";
                        model.IsSuccess = false;
                        return model;
                    }

                    //  wait UpdateNotification(userKRAModel);
                }

                // Find user details
                var user = _dbcontext.Resources
                               .Where(resource => resource.ResourceId == userKRAModels[0].UserId)
                               .Select(resourcename => new
                               {
                                   Resourcename = resourcename.ResourceName,
                                   Email = resourcename.EmailId

                               })
                               .FirstOrDefault();

                if (user != null)
                {
                    var updateTemplate = GetKraUpdateTemplateContent();
                    updateTemplate = updateTemplate.Replace("{userName}", user.Resourcename);

                    // Store notifications to database
                    List<Notification> notifications = await UpdateNotification(userKRAModels, updateTemplate);

                    var header = "Hi  " + user.Resourcename + ",";
                    // if (userKRA.RejectedBy != null)
                    if (notifications != null && notifications.Count > 0)
                    {
                        // Compile kras and send email to user
                        //await SendNotification(notifications, user.Email, Constant.SUBJECT_KRA_CREATED, header);

                        model.IsSuccess = true;
                        model.Messsage = "User KRA Inserted Successfully";
                    }
                    else
                    {
                        model.IsSuccess = false;
                        model.Messsage = "No notifications created";
                    }
                }
                else
                {
                    throw new Exception("Invalid user");
                }

                model.Messsage = "User KRAs Updated Successfully";
                model.IsSuccess = true;
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error: " + ex.Message;
            }

            return model;
        }

        public ResponseModel CreateorUpdateUserKRA(UserKRA userKRAModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                UserKRA userKra;

                if (userKRAModel.QuarterId != null)
                {
                    userKra = _dbcontext.UserKRA.Where(c =>
                        c.UserId == userKRAModel.UserId &&
                        c.KRAId == userKRAModel.KRAId &&
                        c.QuarterId == userKRAModel.QuarterId
                    ).FirstOrDefault();
                }
                else
                {
                    userKra = _dbcontext.UserKRA.Where(c =>
                        c.Id == userKRAModel.Id
                    ).FirstOrDefault();
                }

                if (userKra != null)
                {
                    userKra.Reason = string.IsNullOrEmpty(userKRAModel.Reason) ? userKra.Reason : userKRAModel.Reason;
                    userKra.Comment = string.IsNullOrEmpty(userKRAModel.Comment) ? userKra.Comment : userKRAModel.Comment;
                    userKra.FinalComment = string.IsNullOrEmpty(userKRAModel.FinalComment) ? " " : userKRAModel.FinalComment;
                    userKra.ManagerComment = string.IsNullOrEmpty(userKRAModel.ManagerComment) ? userKra.ManagerComment : userKRAModel.ManagerComment;
                    userKra.DeveloperComment = string.IsNullOrEmpty(userKRAModel.DeveloperComment) ? userKra.DeveloperComment : userKRAModel.DeveloperComment;
                    userKra.ApprovedBy = userKRAModel.ApprovedBy == null ? userKra.ApprovedBy : userKRAModel.ApprovedBy;
                    userKra.RejectedBy = userKRAModel.RejectedBy == null ? userKra.RejectedBy : userKRAModel.RejectedBy;
                    /*
                    userKra.KRAId = userKRAModel.KRAId;
                    userKra.Score = userKRAModel.Score;
                    userKra.Status = userKRAModel.Status;
                    userKra.UserId = userKRAModel.UserId;
                    userKra.QuarterId = userKRAModel.QuarterId;
                    */


                    userKra.IsActive = 1;
                    userKra.UpdateBy = 1;
                    userKra.UpdateDate = DateTime.Now;

                    if (userKRAModel.DeveloperRating != null)
                        userKra.DeveloperRating = userKRAModel.DeveloperRating.Value;

                    if (userKRAModel.ManagerRating != null)
                        userKra.ManagerRating = userKRAModel.ManagerRating.Value;

                    if (userKRAModel.FinalRating != null)
                        userKra.FinalRating = userKRAModel.FinalRating.Value;

                    if (userKRAModel.AppraisalRange != null)
                        userKra.AppraisalRange = userKRAModel.AppraisalRange.Value;

                    _dbcontext.SaveChanges();

                    model.Messsage = "User KRA Update Successfully";
                }
                else
                {
                    userKRAModel.KRAId = userKRAModel.KRAId;
                    userKRAModel.Score = userKRAModel.Score;
                    userKRAModel.Status = userKRAModel.Status;
                    userKRAModel.UserId = userKRAModel.UserId;
                    userKRAModel.Reason = userKRAModel.Reason;
                    userKRAModel.Comment = userKRAModel.Comment;
                    userKRAModel.QuarterId = userKRAModel.QuarterId;
                    userKRAModel.FinalRating = userKRAModel.FinalRating;
                    userKRAModel.FinalComment = userKRAModel.FinalComment;
                    userKRAModel.ManagerRating = userKRAModel.ManagerRating;
                    userKRAModel.AppraisalRange = userKRAModel.AppraisalRange;
                    userKRAModel.DeveloperRating = userKRAModel.DeveloperRating;
                    userKRAModel.ManagerComment = string.IsNullOrEmpty(userKRAModel.DeveloperComment) ? "" : userKRAModel.DeveloperComment;
                    userKRAModel.DeveloperComment = string.IsNullOrEmpty(userKRAModel.DeveloperComment) ? "" : userKRAModel.DeveloperComment;

                    userKRAModel.ApprovedBy = userKRAModel.ApprovedBy;
                    userKRAModel.RejectedBy = userKRAModel.RejectedBy;

                    userKRAModel.IsActive = 1;
                    userKRAModel.CreateBy = 1;
                    userKRAModel.UpdateBy = 1;
                    userKRAModel.CreateDate = DateTime.Now;
                    userKRAModel.UpdateDate = DateTime.Now;

                    _dbcontext.Add(userKRAModel);

                    model.Messsage = "User KRA Inserted Successfully";
                }

                _dbcontext.SaveChanges();

                model.IsSuccess = true;
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }

            return model;
        }

        public async Task<ResponseModel> DeleteUserKRA(int userKRAId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                UserKRA userKra = await GetUserKRAById(userKRAId);

                if (userKra != null)
                {
                    userKra.IsDeleted = 1;

                    _dbcontext.Update(userKra);
                    _dbcontext.SaveChanges();

                    model.IsSuccess = true;
                    model.Messsage = "User KRA Deleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "User KRA Not Found";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }

            return model;
        }

        public List<UserKRADetails> GetKRAsByUserId(int? UserId)
        {
            var userKRADetails = new List<UserKRADetails>();

            try
            {
                userKRADetails = (
                    from kraLibrary in _dbcontext.KRALibrary
                    join userKra in _dbcontext.UserKRA on kraLibrary.Id equals userKra.KRAId
                    join quarter in _dbcontext.QuarterDetails on userKra.QuarterId equals quarter.Id
                    join approver in _dbcontext.Resources on userKra.ApprovedBy equals approver.ResourceId into approverJoin
                    from approver in approverJoin.DefaultIfEmpty()
                    join rejector in _dbcontext.Resources on userKra.RejectedBy equals rejector.ResourceId into rejectorJoin
                    from rejector in rejectorJoin.DefaultIfEmpty()
                    where userKra.UserId == UserId
                    select new UserKRADetails
                    {
                        Id = userKra.Id,
                        KRAId = userKra.KRAId,
                        Score = userKra.Score,
                        Reason = userKra.Reason,
                        UserId = userKra.UserId,
                        Status = userKra.Status.Value,
                        ApprovedBy = userKra.ApprovedBy,
                        RejectedBy = userKra.RejectedBy,
                        QuarterId = (int)userKra.QuarterId,
                        FinalComment = userKra.FinalComment,
                        FinalRating = (int)userKra.FinalRating,
                        ManagerComment = userKra.ManagerComment,
                        ManagerRating = (int)userKra.ManagerRating,
                        DeveloperComment = userKra.DeveloperComment,
                        DeveloperRating = (int)userKra.DeveloperRating,

                        //  ApprovedByName = approver.ResourceName,
                        RejectedByName = rejector.ResourceName,

                        KRAName = kraLibrary.Name,
                        Weightage = kraLibrary.Weightage,
                        WeightageId = kraLibrary.WeightageId,
                        KRADisplayName = kraLibrary.DisplayName,

                        QuarterName = quarter.QuarterName,
                        QuarterYear = quarter.QuarterYear,

                        //StatusName = S.StatusName,
                        //Reason = c.Reason
                    }).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return userKRADetails;
        }

        public List<UserKRARatingList> GetUserKraGraph(int userId, string quarterYearRange)
        {
            try
            {
                var rating = (
                    from project in _dbcontext.Projects
                    join projectResource in _dbcontext.ProjectResources on project.ProjectId equals projectResource.ProjectId
                    join resources in _dbcontext.Resources on projectResource.ResourceId equals resources.ResourceId
                    join userKRA in _dbcontext.UserKRA on resources.ResourceId equals userKRA.UserId
                    join quarterDetail in _dbcontext.QuarterDetails on userKRA.QuarterId equals quarterDetail.Id
                    join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                    join designation in _dbcontext.Designations on resources.DesignationId equals designation.DesignationId
                    where (resources.ResourceId == userId || quarterDetail.QuarterYearRange == quarterYearRange) && quarterDetail.IsActive == 1
                    group new { kraLibrary, userKRA, quarterDetail } by new { quarterDetail.QuarterYear, quarterDetail.QuarterYearRange, quarterDetail.Id, quarterDetail.QuarterName, } into grouped
                    select new
                    {
                        grouped.Key.Id,
                        grouped.Key.QuarterName,
                        grouped.Key.QuarterYear,
                        grouped.Key.QuarterYearRange,
                        Weightage = grouped.Sum(x => x.kraLibrary.Weightage),
                        Score = grouped.Sum(x => x.userKRA.FinalRating * x.kraLibrary.Weightage)
                    }
                    ).ToList();

                var result = rating.Select(r => new UserKRARatingList
                {
                    QuarterYearRange = r.QuarterYearRange,
                    QuarterName = r.QuarterName,
                    Rating = Math.Round((double)r.Score / (double)r.Weightage, 2)
                })
                    .OrderBy(x => x.QuarterYearRange)
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> InsertNotifications(List<Notification> notifications)
        {
            foreach (Notification notification in notifications)
            {
                await _dbcontext.AddAsync(notification);
            }

            await _dbcontext.SaveChangesAsync();

            return true;
        }

        /*
        public async Task<Dictionary<int, Notification>> InsertNotifications(List<UserKRA> userKRAModel, string description)
        {
            Dictionary<int, Notification> notifications = new Dictionary<int, Notification>();

            foreach (var item in userKRAModel)
            {
                Notification notification = new Notification
                {
                    ResourceId = item.UserId.Value,
                    Title = Constant.SUBJECT_KRA_CREATED,
                    Description = description,
                    IsRead = 0,
                    IsActive = 1,
                    CreateAt = DateTime.Now
                };

                // Save notification in database
                await _dbcontext.AddAsync(notification);

                notifications.Add(item.UserId.Value, notification);

                await _dbcontext.SaveChangesAsync();
            }

            return notifications;
        }
        */

        private async Task<bool> SendNotification(UserNotificationData userNotificationData, string templateName)
        {
            string emailContent = string.Empty;
            var subject = Constant.SUBJECT_KRA_CREATED;

            var headerContent = _fileUtil.GetTemplateContent(Constant.KRA_HEADER_TEMPLATE_NAME);
            headerContent = headerContent.Replace("{NAME}", userNotificationData.Name);

            emailContent += headerContent;

            var bodyContent = _fileUtil.GetTemplateContent(templateName);

            StringBuilder builder = new StringBuilder();

            foreach (var notification in userNotificationData.Notifications)
            {
                //builder.Append("<li>" + notification.Description + "</li>");
                builder.Append("<li>" + "KRA Name" + "</li>");
            }

            var KRAName = bodyContent.Replace("{KRA_NAMES}", builder.ToString());

            emailContent += KRAName;

            var footerContent = _fileUtil.GetTemplateContent(Constant.KRA_FOOTER_TEMPLATE_NAME);
            footerContent = footerContent.Replace("{CREATE_DATE}", DateTime.Now.ToString());

            emailContent += footerContent;

            await _emailService.SendEmail(userNotificationData.Email, subject, emailContent);

            return true;
        }

        /*
        private async Task<bool> SendNotification(List<Notification> notifications, string email, string subject, string header)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(header);
            builder.Append("<br/>");

            foreach (var notification in notifications)
            {
                builder.Append(notification.Description);
                //builder.Append("<br/>");
            }

            builder.Append("Thanks & Regards,");
            builder.Append("<br>");
            builder.Append("Datafortune Software Solutions Pvt. Ltd.");

            await _emailService.SendEmail(email, subject, builder.ToString());

            return true;
        }
        */

        public async Task<List<Notification>> UpdateNotification(List<UserKRA> userKRAModel, string description)
        {
            /**
             * KRA can be updated by following conditions:
             * 1. Admin approved
             * 2. Admin rejected
             * 
             * 3. Manager approved
             * 4. Manager rejected
             * 
             * 5. Developer rated
             */

            List<Notification> notifications = new List<Notification>();

            string emailContent = string.Empty;
            Notification notification = new Notification();

            // If kra is rejected

            foreach (var userKRA in userKRAModel)
            {
                

                if (userKRA.RejectedBy != null)
                {
                    // emailContent = GetKraRejectedTemplateContent().Replace("{username}", user.Resourcename);
                    notification.ResourceId = userKRA.UserId.Value;
                    notification.Description = description;
                    notification.Title = Constant.KRA_UPDATE_MANAGER_REJECTED;
                }
                else
                {
                    if (userKRA.FinalRating != 0 || userKRA.ManagerRating != 0 || userKRA.DeveloperRating != 0)
                    {
                        // emailContent = GetKraUpdateTemplateContent().Replace("{username}", user.Resourcename);
                        notification.ResourceId = userKRA.UserId.Value;
                        notification.Title = "Kra Updated";
                        notification.Description = description;
                    }
                }
            }

           // notification.ResourceId = userKRAModel.UserId.Value;
            notification.IsRead = 0;
            notification.IsActive = 1;
            notification.CreateAt = DateTime.Now;

          //  await _emailService.SendEmail(user.Email, Constant.SUBJECT_KRA_UPDATED, emailContent);
            await _dbcontext.AddAsync(notification);
            notifications.Add(notification);
            await _dbcontext.SaveChangesAsync();
            return notifications;
        }

        public string GetKraUpdateTemplateContent()
        {
            var templateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Templates", "kra-updated.html");
            if (File.Exists(templateFilePath))
            {
                return File.ReadAllText(templateFilePath);
            }
            throw new FileNotFoundException("kra-updated.html not found.");
        }

        public string GetKraCreatedTemplateContent()
        {
            var templateFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Templates", "kra-created.html");

            if (File.Exists(templateFilePath))
            {
                return File.ReadAllText(templateFilePath);
            }

            throw new FileNotFoundException("kra-created.html not found.");
        }
    }
}

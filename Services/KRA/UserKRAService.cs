﻿using System;
using System.Linq;
using DF_PA_API.Models;
//using System.Data.Entity;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using DF_EvolutionAPI.Services.KRA;
using Microsoft.EntityFrameworkCore;
using DF_EvolutionAPI.Services.Email;
using DF_EvolutionAPI.Models.Response;
using Microsoft.Extensions.Logging;


namespace DF_EvolutionAPI.Services
{
    public class UserKRAService : IUserKRAService
    {
        private readonly FileUtil _fileUtil;
        private readonly IEmailService _emailService;
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<UserKRAService> _logger;

        public UserKRAService(DFEvolutionDBContext dbContext, IWebHostEnvironment hostingEnvironment, IEmailService emailService, FileUtil fileUtil, ILogger<UserKRAService> logger)
        {
            _fileUtil = fileUtil;
            _dbcontext = dbContext;
            _emailService = emailService;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public async Task<List<UserKRA>> GetAllUserKRAList()
        {
            return await _dbcontext.UserKRA.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<UserKRA> GetUserKRAById(int userKRAId)
        {
            UserKRA userKRA;
            try
            {
                userKRA = await _dbcontext.UserKRA.Where(c => c.IsActive == (int)Status.IS_ACTIVE && c.Id == userKRAId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw; ;
            }
        }

        //Displaying all kra's according to designation Id.
        public List<UserAssignedKRA> GetAssignedKRAsByDesignationId(int designationId)
        {
            try
            {
                var query = from resource in _dbcontext.Resources
                            join userKRA in _dbcontext.UserKRA on resource.ResourceId equals userKRA.UserId
                            join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                            where resource.DesignationId == designationId && userKRA.IsActive == (int)Status.IS_ACTIVE
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }

        public async Task<ResponseModel> CreateUserKRA(List<UserKRA> userKRAModel)
        {
            /**
             * This method performs following actions:
             * 1. Run loop on passed kras and store in database
             * 2. Create/store notifications while finding user and reading template
             * 3. For each user => notifications map, send one single email pser user
             */

            ResponseModel model = new ResponseModel();

            bool created = await CreateKraEntries(userKRAModel);
            if (created)
            {
                Dictionary<int, UserNotificationData> notificationMap = await CreateNotifications(userKRAModel);

                SendNotificationsAsync(notificationMap);

                model.IsSuccess = true;
            }
            else
            {
                model.IsSuccess = false;
            }

            return model;
        }

        //Asynchronously sends notifications to users in a fire-and-forget manner.
        private void SendNotificationsAsync(Dictionary<int, UserNotificationData> notificationMap)
        {
            _ = Task.Run(async () =>
            {
                foreach (var entry in notificationMap)
                {
                    await SendNotification(entry.Value, Constant.KRA_CREATED_TEMPLATE_NAME);
                }
            });
        }

        public async Task<bool> CreateKraEntries(List<UserKRA> userKRAModel)
        {
            try
            {

                foreach (var item in userKRAModel)
                {
                    //To restrict the duplicate entries of kras for particular quarter and user. 'var kralist'
                    var kralist = _dbcontext.UserKRA.Where(kra =>
                     //kra.QuarterId == item.QuarterId && kra.KRAId == item.KRAId && kra.UserId == item.UserId && kra.IsActive == (int)Status.IS_ACTIVE).ToList();
                     kra.QuarterId == item.QuarterId && kra.UserId == item.UserId && kra.IsActive == (int)Status.IS_ACTIVE).ToList();
                    
                    if (kralist.Count == 0)
                    {
                        item.ManagerComment = string.IsNullOrEmpty(item.DeveloperComment) ? "" : item.DeveloperComment;
                        item.DeveloperComment = string.IsNullOrEmpty(item.DeveloperComment) ? "" : item.DeveloperComment;
                        item.ApprovedBy = item.ApprovedBy == null ? item.ApprovedBy : item.ApprovedBy;
                        item.RejectedBy = item.RejectedBy == null ? item.RejectedBy : item.RejectedBy;
                        item.IsApproved = 0;
                        item.IsActive = (int)Status.IS_ACTIVE;
                        item.CreateBy = item.CreateBy;
                        item.CreateDate = DateTime.Now;
                        item.UpdateDate = DateTime.Now;

                        await _dbcontext.AddAsync(item);
                    }
                    else
                    {
                        return false;
                    }
                }

                await _dbcontext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
            catch(Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
                var user = _dbcontext.Resources
                               .Where(resource => resource.ResourceId == userKRA.UserId.Value)
                               .FirstOrDefault();

                Notification notification = new Notification
                {
                    ResourceId = userKRA.UserId.Value,
                    Title = Constant.SUBJECT_KRA_CREATED,
                    Description = userKRA.KRAName,     // store kra name
                    IsRead = 0,
                    IsActive = (int)Status.IS_ACTIVE,
                    CreateAt = DateTime.Now
                };

                notificationMap[userKRA.UserId.Value].Email = user.EmailId;
                notificationMap[userKRA.UserId.Value].ManagerName = user.ResourceName;
                notificationMap[userKRA.UserId.Value].Notifications.Add(notification);
            }

            return notificationMap;
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
        private async Task<bool> SendNotification(UserNotificationData userNotificationData, string templateName)
        {
            if (string.IsNullOrEmpty(userNotificationData.Email))
            {
                // Log or handle the case where the email is blank
                Console.WriteLine("Email is blank. Notification not sent.");
                return false; // Return false if no email is sent
            }

            bool isKraUpdated = userNotificationData.Notifications
                .Any(n => n.Title == (userNotificationData.IsForSrManager? Constant.SUBJECT_KRA_UPDATED_SRMANAGER: Constant.SUBJECT_KRA_UPDATED_MANAGER));

            string subject = isKraUpdated ? (userNotificationData.IsForSrManager ? Constant.SUBJECT_KRA_UPDATED_SRMANAGER : Constant.SUBJECT_KRA_UPDATED_MANAGER) : Constant.SUBJECT_KRA_CREATED;
            string headerTemplate = isKraUpdated ? (userNotificationData.IsForSrManager ? Constant.KRA_HEADER_SR_APPROVED_TEMPLATE_NAME : Constant.KRA_HEADER_APPROVED_TEMPLATE_NAME) : Constant.KRA_HEADER_TEMPLATE_NAME;

            // Fetch and format header content
            string headerContent = _fileUtil.GetTemplateContent(headerTemplate)
                .Replace("{NAME}", userNotificationData.ManagerName)
                .Replace("{UserName}", userNotificationData.UserName)
                .Replace("{ManagerName}", userNotificationData.SrManagerName);

            // Fetch and format footer content
            string footerContent = _fileUtil.GetTemplateContent(Constant.KRA_FOOTER_TEMPLATE_NAME)
                .Replace("{CREATE_DATE}", DateTime.Now.ToString());

            // Combine email content
            string emailContent = $"{headerContent}{footerContent}";

           
            await _emailService.SendEmail(userNotificationData.Email, subject, emailContent);

            return true;
        }

        private List<int> GetUserIds(List<UserKRA> userKRAModel)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> UpdateUserKra(UpdateUserKRARequest request)
        {
            /**
             * This method performs following actions:
             * 1. Run loop on passed kras and store in database
             * 2. Update/store notifications while finding user and reading template
             * 3. For each user => notifications map, send one single email pser user
             */
            ResponseModel model = new ResponseModel();

            bool created = await UpdateKraEntries(request.UserKRAModel, request.UserAchievement, request.ManagerQuartelyComment);
            if (created)
            {
                Dictionary<int, UserNotificationData> notificationMap = await CreateUpdateNotifications(request.UserKRAModel);
                SendNotificationsAsync(notificationMap);

                model.IsSuccess = true;
            }
            else
            {
                model.IsSuccess = false;
            }
            return model;
        }

        public async Task<bool> UpdateKraEntries(List<UserKRA> userKRAModel, string userAchievement, string managerQuartelyComment)
        {
            try
            {
                if (userKRAModel == null || !userKRAModel.Any())
                    return false; // No data to process

                var firstItem = userKRAModel.First(); // Get user and quarter details
                int userId = firstItem.UserId.Value;
                int quarterId = firstItem.QuarterId.Value;
                int createdBy = firstItem.UserId.Value;

                foreach (var userKRAModels in userKRAModel)
                {
                    var userKra = await _dbcontext.UserKRA.FindAsync(userKRAModels.Id);
                    if (userKra != null)
                    {
                        var weightage = _dbcontext.KRALibrary
                            .Where(k => k.Id == userKra.KRAId)
                            .Select(k => k.Weightage)
                            .FirstOrDefault();

                        userKra.Reason = userKRAModels.Reason;
                        userKra.Comment = userKRAModels.Comment;
                        userKra.ApprovedBy = userKRAModels.ApprovedBy;
                        userKra.RejectedBy = userKRAModels.RejectedBy;
                        userKra.FinalComment = userKRAModels.FinalComment;
                        userKra.ManagerComment = userKRAModels.ManagerComment;
                        userKra.FinalRating = userKRAModels.FinalRating;
                        userKra.DeveloperComment = userKRAModels.DeveloperComment;
                        userKra.ManagerRating = userKRAModels.ManagerRating;
                        userKra.AppraisalRange = userKRAModels.AppraisalRange;
                        userKra.DeveloperRating = userKRAModels.DeveloperRating;
                        userKra.IsApproved = userKRAModels.IsApproved ?? 0;
                        userKra.IsActive = userKRAModels.IsActive;
                        userKra.UpdateBy = userKRAModels.UpdateBy;
                        userKra.UpdateDate = DateTime.Now;

                        if (userKRAModels.ManagerRating != null && userKRAModels.FinalRating.HasValue && weightage != 0)
                        {
                            userKra.Score = (double)userKRAModels.FinalRating * (double)weightage;
                        }
                        else
                        {
                            userKra.Score = 0;
                        }

                        _dbcontext.UserKRA.Update(userKra);
                    }
                }

                // Check if there is at least one non-empty comment before proceeding
                if (!string.IsNullOrEmpty(userAchievement) || !string.IsNullOrEmpty(managerQuartelyComment))
                {
                    var existingComment =_dbcontext.UserQuarterlyAchievements
                        .FirstOrDefault(c => c.UserId == userId && c.QuarterId == quarterId && c.IsActive == (int)Status.IS_ACTIVE);

                    if (existingComment == null)
                    {
                        // Insert new record if no existing comment is found
                        var newAchievementComment = new UserQuarterlyAchievement
                        {
                            UserId = userId,
                            QuarterId = quarterId,
                            UserAchievement = userAchievement,
                            ManagerQuartelyComment = managerQuartelyComment,
                            CreateBy = createdBy,
                            CreateDate = DateTime.Now,
                            IsActive = 1
                        };
                        _dbcontext.UserQuarterlyAchievements.Add(newAchievementComment);
                    }
                    else
                    {
                        // Update only if an existing record is found
                        bool isUpdated = false;

                        if (!string.IsNullOrEmpty(userAchievement))
                        {
                            existingComment.UserAchievement = userAchievement;

                            isUpdated = true;
                        }
                        if (!string.IsNullOrEmpty(managerQuartelyComment))
                        {
                            existingComment.ManagerQuartelyComment = managerQuartelyComment;
                            isUpdated = true;
                        }

                        if (isUpdated)
                        {
                            existingComment.UpdateBy = userId;
                            existingComment.UpdateDate = DateTime.Now;
                            _dbcontext.UserQuarterlyAchievements.Update(existingComment);
                        }
                    }

                    await _dbcontext.SaveChangesAsync(); // Ensure data is saved
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));                
                throw;
            }
        }


        private async Task<Dictionary<int, UserNotificationData>> PrepareUpdateNotifications(List<UserKRA> userKRAModel)
        {
            Dictionary<int, UserNotificationData> notificationMap = new Dictionary<int, UserNotificationData>();

            // **Check if any KRA has FinalRating == 0**
            bool hasPendingFinalRating = false;
            var finalrating = userKRAModel.Count(kra => kra.FinalRating == null || kra.FinalRating == 0);
            if( finalrating == 0 )
            {
                hasPendingFinalRating = true;
            }

            foreach (UserKRA userKRA in userKRAModel)
            {
                if (!notificationMap.ContainsKey(userKRA.UserId.Value))
                {
                    UserNotificationData userNotificationData = new UserNotificationData();
                    userNotificationData.Notifications = new List<Notification>();

                    notificationMap[userKRA.UserId.Value] = userNotificationData;

                }

                // Find user details
                var user = _dbcontext.Resources
                               .Where(resource => resource.ResourceId == userKRA.UserId.Value)
                               .FirstOrDefault();

                Notification notification = new Notification
                {
                    ResourceId = userKRA.UserId.Value,
                  // Title = Constant.SUBJECT_KRA_UPDATED_MANAGER,
                    Description = userKRA.KRAName,     // store kra name
                    IsRead = 0,
                    IsActive = (int)Status.IS_ACTIVE,
                    CreateAt = DateTime.Now
                };

                if (userKRA.isUpdated == true)
                {

                    //Fetching the manager details.                    
                    var userName = _dbcontext.Resources.Where(resources => resources.ResourceId == userKRA.UserId.Value).FirstOrDefault();
                    var managerDetails = _dbcontext.Resources.Where(resources => resources.ResourceId == userName.ReportingTo.Value).FirstOrDefault();                   
                    var srManagerDetails = (from r in _dbcontext.Resources
                                            join d in _dbcontext.Designations
                                            on r.DesignationId equals d.DesignationId
                                            where r.ResourceId == managerDetails.ReportingTo.Value &&                                           
                                            !Constant.NO_MAIL_DESIGNATION.Contains(d.DesignationName)
                                            select new { r.ResourceName, r.EmailId })
                                            .FirstOrDefault();

                    //Sending mail according to manager after user has submitted their rating.
                    if ((userKRA.ManagerRating == null || userKRA.ManagerRating == 0)
                        && (userKRA.RejectedBy == null || userKRA.RejectedBy == 0))
                    {
                        notificationMap[userKRA.UserId.Value].Email = managerDetails.EmailId;
                        notificationMap[userKRA.UserId.Value].ManagerName = managerDetails.ResourceName;
                        notificationMap[userKRA.UserId.Value].UserName = userName.ResourceName;
                    }

                    // For Rejection mail to user.
                    //else if (userKRA.RejectedBy != null && userKRA.RejectedBy != 0
                    //         && (userKRA.ManagerRating == null || userKRA.ManagerRating == 0))
                    //{
                    //    notificationMap[userKRA.UserId.Value].Email = user.EmailId;
                    //    notificationMap[userKRA.UserId.Value].ManagerName = reportingTos.ResourceName;
                    //    notificationMap[userKRA.UserId.Value].UserName = managerDetails.ResourceName;
                    //}

                    else if (hasPendingFinalRating == true && (userKRA.IsApproved == null || userKRA.IsApproved == 0))
                    {
                        notificationMap[userKRA.UserId.Value].Email = srManagerDetails?.EmailId;
                        notificationMap[userKRA.UserId.Value].ManagerName = srManagerDetails?.ResourceName;
                        notificationMap[userKRA.UserId.Value].UserName = userName.ResourceName;
                        notificationMap[userKRA.UserId.Value].SrManagerName = managerDetails.ResourceName;
                        notificationMap[userKRA.UserId.Value].IsForSrManager = true;
                    }

                    notification.Title = notificationMap[userKRA.UserId.Value].IsForSrManager
                                         ? Constant.SUBJECT_KRA_UPDATED_SRMANAGER
                                         : Constant.SUBJECT_KRA_UPDATED_MANAGER;


                    notificationMap[userKRA.UserId.Value].Notifications.Add(notification);
                }
            }
            return notificationMap;
        }

        public async Task<Dictionary<int, UserNotificationData>> CreateUpdateNotifications(List<UserKRA> userKRAModel)
        {
            try
            {
                Dictionary<int, UserNotificationData> notificationMap = await PrepareUpdateNotifications(userKRAModel);

                foreach (var entry in notificationMap)
                {
                    await InsertNotifications(entry.Value.Notifications);
                }

                return notificationMap;
            }
            catch(Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }

        public async Task<ResponseModel> DeleteUserKRA(int userKRAId)
        {
            ResponseModel model = new ResponseModel();
            UserKRA userKra = null;
            try
            {
                userKra = _dbcontext.UserKRA.Where(c => c.IsActive == (int)Status.IS_ACTIVE && c.Id == userKRAId).FirstOrDefault();

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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
                join userAchievement in _dbcontext.UserQuarterlyAchievements
                    on new { userKra.UserId, userKra.QuarterId } equals new { userAchievement.UserId, userAchievement.QuarterId }
                    into achievementJoin
                from userAchievement in achievementJoin.Where(a => a.IsActive == (int)Status.IS_ACTIVE).DefaultIfEmpty()
                join managerComment in _dbcontext.UserQuarterlyAchievements
                    on new { userKra.UserId, userKra.QuarterId } equals new { managerComment.UserId, managerComment.QuarterId }
                    into managerCommentJoin
                from managerComment in managerCommentJoin.Where(m => m.IsActive == (int)Status.IS_ACTIVE).DefaultIfEmpty()
                where userKra.UserId == UserId && kraLibrary.IsActive == (int)Status.IS_ACTIVE && userKra.IsActive == (int)Status.IS_ACTIVE
                select new UserKRADetails
                {
                    Id = userKra.Id,
                    KRAId = userKra.KRAId,
                    Score = userKra.Score,
                    Reason = userKra.Reason,
                    UserId = userKra.UserId,
                    Status = userKra.Status.Value,
                    IsSpecial = kraLibrary.IsSpecial,
                    ApprovedBy = userKra.ApprovedBy,
                    RejectedBy = userKra.RejectedBy,
                    QuarterId = (int)userKra.QuarterId,
                    FinalComment = userKra.FinalComment,
                    FinalRating = userKra.FinalRating,
                    ManagerComment = userKra.ManagerComment,
                    ManagerRating = userKra.ManagerRating,
                    DeveloperComment = userKra.DeveloperComment,
                    DeveloperRating = userKra.DeveloperRating,
                    RejectedByName = rejector.ResourceName,
                    ApprovedByName = approver.ResourceName,
                    KRAName = kraLibrary.Name,
                    Weightage = kraLibrary.Weightage,
                    WeightageId = kraLibrary.WeightageId,
                    KRADisplayName = kraLibrary.DisplayName,
                    IsDescriptionRequired = kraLibrary.IsDescriptionRequired,
                    MinimumRatingForDescription = kraLibrary.MinimumRatingForDescription,
                    QuarterName = quarter.QuarterName,
                    QuarterYear = quarter.QuarterYear,
                    IsActive = userKra.IsActive,
                    Description = kraLibrary.Description,
                    IsApproved = userKra.IsApproved,
                    UserAchievement = userAchievement != null ? userAchievement.UserAchievement : null,
                    ManagerQuartelyComment = managerComment != null ? managerComment.ManagerQuartelyComment : null
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return userKRADetails;
        }

        public List<UserKRARatingList> GetUserKraGraph(int userId, string quarterYearRange)
        {
            try
            {
                var rating = (
                    from resources in _dbcontext.Resources
                    join userKRA in _dbcontext.UserKRA on resources.ResourceId equals userKRA.UserId
                    join quarterDetail in _dbcontext.QuarterDetails on userKRA.QuarterId equals quarterDetail.Id
                    join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                    join designation in _dbcontext.Designations on resources.DesignationId equals designation.DesignationId
                    where (resources.ResourceId == userId && quarterDetail.QuarterYearRange == quarterYearRange && quarterDetail.IsActive == (int)Status.IS_ACTIVE && userKRA.IsActive == (int)Status.IS_ACTIVE && userKRA.IsApproved != 0)
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return new List<UserKRARatingList>();
            }
        }

        //UnassignKra is used to removed the assigned kras from resources.
        public async Task<ResponseModel> AssignUnassignKra(int userKraId, byte IsActive)
        {
            ResponseModel model = new ResponseModel();
            UserKRA userKra = null;
            try
            {
                userKra = _dbcontext.UserKRA.Where(c => c.Id == userKraId).FirstOrDefault();
                if (userKra != null)
                {
                    userKra.IsActive = IsActive;

                    _dbcontext.Update(userKra);
                    _dbcontext.SaveChanges();

                    model.IsSuccess = true;
                    if (userKra.IsActive == (int)Status.IN_ACTIVE)
                    {
                        model.Messsage = "User KRA Unassigned Successfully";
                    }
                    else
                    {
                        model.Messsage = "User KRA Assigned Successfully";
                    }
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "User KRA does not exits";
                }

            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }

        /// Get the list of Resources whoes kras are released.
        public async Task<List<UserKRA>> GetReleasedKraUsers(int quarterId, int managerId)
        {
            try
            {
                var userKRAList = await _dbcontext.UserKRA
                    .Where(c => c.IsActive == (int)Status.IS_ACTIVE
                                && c.QuarterId == quarterId
                                && c.CreateBy == managerId)
                    .ToListAsync();

                return userKRAList;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

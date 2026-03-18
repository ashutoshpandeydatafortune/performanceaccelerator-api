using System;
using System.Linq;
using DF_PA_API.Models;
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

        private void SendUpdateNotificationsAsync(Dictionary<int, UserNotificationData> notificationMap)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("SendUpdateNotificationsAsync started. Processing {Count} users", notificationMap.Count);
                    
                    foreach (var entry in notificationMap)
                    {
                        _logger.LogInformation("Processing notifications for UserId: {UserId}, Total notifications: {Count}", 
                            entry.Key, entry.Value.Notifications.Count);
                        
                        var notifications = entry.Value.Notifications;
                        
                        var approvalCompleteNotifications = notifications.Where(n => n.Title == Constant.SUBJECT_KRA_APPROVED_COMPLETE).ToList();
                        var rejectionNotifications = notifications.Where(n => n.Title == Constant.SUBJECT_KRA_REJECTED).ToList();
                        var managerNotifications = notifications.Where(n => n.Title == Constant.SUBJECT_KRA_UPDATED_MANAGER).ToList();
                        var srManagerNotifications = notifications.Where(n => n.Title == Constant.SUBJECT_KRA_UPDATED_SRMANAGER).ToList();

                        _logger.LogInformation("UserId {UserId} - Approval:{Approval}, Rejection:{Rejection}, Manager:{Manager}, SrManager:{SrManager}", 
                            entry.Key, approvalCompleteNotifications.Count, rejectionNotifications.Count, 
                            managerNotifications.Count, srManagerNotifications.Count);

                        if (approvalCompleteNotifications.Any())
                        {
                            _logger.LogInformation("Sending approval complete email to {Email}", entry.Value.UserEmail);
                            var approvalCompleteData = new UserNotificationData
                            {
                                Email = entry.Value.UserEmail,
                                UserName = entry.Value.UserName,
                                ManagerName = entry.Value.ManagerName,
                                SrManagerName = entry.Value.SrManagerName,
                                IsApprovalComplete = true,
                                Notifications = approvalCompleteNotifications
                            };
                            await SendNotification(approvalCompleteData, Constant.KRA_HEADER_APPROVAL_COMPLETE_TEMPLATE_NAME);
                        }

                        if (rejectionNotifications.Any())
                        {
                            _logger.LogInformation("Sending rejection email to {Email}", entry.Value.UserEmail);
                            var rejectionData = new UserNotificationData
                            {
                                Email = entry.Value.UserEmail,
                                UserName = entry.Value.UserName,
                                ManagerName = entry.Value.RejectedByManagerName,
                                RejectionReason = entry.Value.RejectionReason,
                                IsRejection = true,
                                Notifications = rejectionNotifications
                            };
                            await SendNotification(rejectionData, Constant.KRA_HEADER_REJECTED_TEMPLATE_NAME);
                        }

                        if (managerNotifications.Any())
                        {
                            _logger.LogInformation("Sending manager notification email to {Email}", entry.Value.ManagerEmail);
                            var managerData = new UserNotificationData
                            {
                                Email = entry.Value.ManagerEmail,
                                UserName = entry.Value.UserName,
                                ManagerName = entry.Value.ManagerName,
                                Notifications = managerNotifications
                            };
                            await SendNotification(managerData, Constant.KRA_HEADER_APPROVED_TEMPLATE_NAME);
                        }

                        if (srManagerNotifications.Any())
                        {
                            _logger.LogInformation("Sending SR manager notification email to {Email}", entry.Value.SrManagerEmail);
                            var srManagerData = new UserNotificationData
                            {
                                Email = entry.Value.SrManagerEmail,
                                UserName = entry.Value.UserName,
                                ManagerName = entry.Value.ManagerName,
                                SrManagerName = entry.Value.SrManagerName,
                                IsForSrManager = true,
                                Notifications = srManagerNotifications
                            };
                            await SendNotification(srManagerData, Constant.KRA_HEADER_SR_APPROVED_TEMPLATE_NAME);
                        }
                    }
                    
                    _logger.LogInformation("SendUpdateNotificationsAsync completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SendUpdateNotificationsAsync");
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
            try
            {
                if (string.IsNullOrEmpty(userNotificationData.Email))
                {
                    _logger.LogWarning("Email is blank. Notification not sent.");
                    return false;
                }

                bool isApprovalComplete = userNotificationData.IsApprovalComplete;
                bool isKraRejected = userNotificationData.IsRejection;
                bool isKraUpdated = userNotificationData.Notifications
                    .Any(n => n.Title == (userNotificationData.IsForSrManager? Constant.SUBJECT_KRA_UPDATED_SRMANAGER: Constant.SUBJECT_KRA_UPDATED_MANAGER));

                string subject;
                string headerTemplate;

                if (isApprovalComplete)
                {
                    subject = Constant.SUBJECT_KRA_APPROVED_COMPLETE;
                    headerTemplate = Constant.KRA_HEADER_APPROVAL_COMPLETE_TEMPLATE_NAME;
                }
                else if (isKraRejected)
                {
                    subject = Constant.SUBJECT_KRA_REJECTED;
                    headerTemplate = Constant.KRA_HEADER_REJECTED_TEMPLATE_NAME;
                }
                else if (isKraUpdated)
                {
                    subject = userNotificationData.IsForSrManager ? Constant.SUBJECT_KRA_UPDATED_SRMANAGER : Constant.SUBJECT_KRA_UPDATED_MANAGER;
                    headerTemplate = userNotificationData.IsForSrManager ? Constant.KRA_HEADER_SR_APPROVED_TEMPLATE_NAME : Constant.KRA_HEADER_APPROVED_TEMPLATE_NAME;
                }
                else
                {
                    subject = Constant.SUBJECT_KRA_CREATED;
                    headerTemplate = Constant.KRA_HEADER_TEMPLATE_NAME;
                }

                _logger.LogInformation("Sending email - To: {Email}, Subject: {Subject}, Template: {Template}", 
                    userNotificationData.Email, subject, headerTemplate);

                string headerContent = _fileUtil.GetTemplateContent(headerTemplate)
                    .Replace("{NAME}", userNotificationData.UserName ?? userNotificationData.ManagerName)
                    .Replace("{UserName}", userNotificationData.UserName)
                    .Replace("{ManagerName}", userNotificationData.ManagerName)
                    .Replace("{SrManagerName}", userNotificationData.SrManagerName)
                    .Replace("{REASON}", userNotificationData.RejectionReason ?? "No reason provided")
                    .Replace("{DUE_DATE}", Constant.DUE_DATE);

                string footerContent = _fileUtil.GetTemplateContent(Constant.KRA_FOOTER_TEMPLATE_NAME)
                    .Replace("{CREATE_DATE}", DateTime.Now.ToString());

                string emailContent = $"{headerContent}{footerContent}";

                await _emailService.SendEmail(userNotificationData.Email, subject, emailContent);
                
                _logger.LogInformation("Email sent successfully to {Email}", userNotificationData.Email);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", userNotificationData.Email);
                return false;
            }
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
                SendUpdateNotificationsAsync(notificationMap);

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
            _logger.LogInformation("PrepareUpdateNotifications started with {Count} KRAs", userKRAModel.Count);
            
            Dictionary<int, UserNotificationData> notificationMap = new Dictionary<int, UserNotificationData>();

            bool hasPendingFinalRating = false;
            var finalrating = userKRAModel.Count(kra => kra.FinalRating == null || kra.FinalRating == 0);
            if( finalrating == 0 )
            {
                hasPendingFinalRating = true;
            }

            _logger.LogInformation("hasPendingFinalRating: {HasPending}, KRAs without final rating: {Count}", 
                hasPendingFinalRating, finalrating);

            var groupedByUser = userKRAModel.GroupBy(k => k.UserId.Value);

            foreach (var userGroup in groupedByUser)
            {
                int userId = userGroup.Key;
                
                _logger.LogInformation("Processing UserId: {UserId}, KRA count: {Count}", userId, userGroup.Count());
                
                if (!notificationMap.ContainsKey(userId))
                {
                    UserNotificationData userNotificationData = new UserNotificationData();
                    userNotificationData.Notifications = new List<Notification>();
                    notificationMap[userId] = userNotificationData;
                }

                var user = _dbcontext.Resources.Where(resource => resource.ResourceId == userId).FirstOrDefault();
                var managerDetails = _dbcontext.Resources.Where(resources => resources.ResourceId == user.ReportingTo.Value).FirstOrDefault();
                var srManagerDetails = (from r in _dbcontext.Resources
                                        join d in _dbcontext.Designations
                                        on r.DesignationId equals d.DesignationId
                                        where r.ResourceId == managerDetails.ReportingTo.Value &&
                                        !Constant.NO_MAIL_DESIGNATION.Contains(d.DesignationName)
                                        select new { r.ResourceName, r.EmailId, r.ResourceId })
                                        .FirstOrDefault();

                _logger.LogInformation("User: {User}, Manager: {Manager}, SrManager: {SrManager}, SrManagerEmail: {SrEmail}", 
                    user?.ResourceName, managerDetails?.ResourceName, srManagerDetails?.ResourceName, srManagerDetails?.EmailId);

                notificationMap[userId].UserEmail = user.EmailId;
                notificationMap[userId].ManagerEmail = managerDetails.EmailId;
                notificationMap[userId].SrManagerEmail = srManagerDetails?.EmailId;
                notificationMap[userId].UserName = user.ResourceName;
                notificationMap[userId].ManagerName = managerDetails.ResourceName;
                notificationMap[userId].SrManagerName = srManagerDetails?.ResourceName;

                var rejectedKras = userGroup.Where(k => k.isUpdated == true && k.RejectedBy != null && k.RejectedBy != 0).ToList();
                var managerRatedKras = userGroup.Where(k => k.isUpdated == true && k.RejectedBy == null && (k.ManagerRating == null || k.ManagerRating == 0)).ToList();
                var srManagerKras = userGroup.Where(k => k.isUpdated == true && k.RejectedBy == null && hasPendingFinalRating && (k.IsApproved == null || k.IsApproved == 0)).ToList();
                var approvedKras = userGroup.Where(k => k.isUpdated == true && k.IsApproved == 1 && k.FinalRating != null && k.FinalRating != 0).ToList();

                _logger.LogInformation("UserId {UserId} KRA breakdown - Rejected:{Rejected}, ManagerRated:{Manager}, SrManager:{SrMgr}, Approved:{Approved}", 
                    userId, rejectedKras.Count, managerRatedKras.Count, srManagerKras.Count, approvedKras.Count);

                bool allKrasApproved = approvedKras.Any() && 
                                      !rejectedKras.Any() && 
                                      !managerRatedKras.Any() && 
                                      !srManagerKras.Any() &&
                                      approvedKras.Count == userGroup.Count(k => k.isUpdated == true);

                if (allKrasApproved)
                {
                    _logger.LogInformation("All KRAs approved for UserId: {UserId}", userId);
                    notificationMap[userId].IsApprovalComplete = true;
                    
                    Notification completionNotification = new Notification
                    {
                        ResourceId = userId,
                        Title = Constant.SUBJECT_KRA_APPROVED_COMPLETE,
                        Description = "All KRAs approved",
                        IsRead = 0,
                        IsActive = (int)Status.IS_ACTIVE,
                        CreateAt = DateTime.Now
                    };
                    notificationMap[userId].Notifications.Add(completionNotification);
                }

                foreach (var userKRA in rejectedKras)
                {
                    _logger.LogInformation("Creating rejection notification for KRA: {KraName}", userKRA.KRAName);
                    
                    Notification notification = new Notification
                    {
                        ResourceId = userId,
                        Title = Constant.SUBJECT_KRA_REJECTED,
                        Description = userKRA.KRAName,
                        IsRead = 0,
                        IsActive = (int)Status.IS_ACTIVE,
                        CreateAt = DateTime.Now
                    };

                    notificationMap[userId].Email = user.EmailId;
                    notificationMap[userId].RejectionReason = userKRA.Reason;
                    notificationMap[userId].IsRejection = true;
                    
                    if (userKRA.RejectedBy == srManagerDetails?.ResourceId)
                    {
                        notificationMap[userId].RejectedByManagerName = srManagerDetails?.ResourceName;
                        _logger.LogInformation("Rejected by SR Manager: {SrManager}", srManagerDetails?.ResourceName);
                    }
                    else
                    {
                        notificationMap[userId].RejectedByManagerName = managerDetails.ResourceName;
                        _logger.LogInformation("Rejected by Manager: {Manager}", managerDetails.ResourceName);
                    }
                    
                    notificationMap[userId].Notifications.Add(notification);
                }

                foreach (var userKRA in managerRatedKras)
                {
                    _logger.LogInformation("Creating manager notification for KRA: {KraName}", userKRA.KRAName);
                    
                    Notification notification = new Notification
                    {
                        ResourceId = userId,
                        Title = Constant.SUBJECT_KRA_UPDATED_MANAGER,
                        Description = userKRA.KRAName,
                        IsRead = 0,
                        IsActive = (int)Status.IS_ACTIVE,
                        CreateAt = DateTime.Now
                    };

                    notificationMap[userId].Notifications.Add(notification);
                }

                foreach (var userKRA in srManagerKras)
                {
                    _logger.LogInformation("Creating SR manager notification for KRA: {KraName}, IsApproved: {IsApproved}", 
                        userKRA.KRAName, userKRA.IsApproved);
                    
                    Notification notification = new Notification
                    {
                        ResourceId = userId,
                        Title = Constant.SUBJECT_KRA_UPDATED_SRMANAGER,
                        Description = userKRA.KRAName,
                        IsRead = 0,
                        IsActive = (int)Status.IS_ACTIVE,
                        CreateAt = DateTime.Now
                    };

                    notificationMap[userId].IsForSrManager = true;
                    notificationMap[userId].Notifications.Add(notification);
                }
            }
            
            _logger.LogInformation("PrepareUpdateNotifications completed. Created notifications for {Count} users", notificationMap.Count);
            
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
                //where userKra.UserId == UserId && kraLibrary.IsActive == (int)Status.IS_ACTIVE && userKra.IsActive == (int)Status.IS_ACTIVE
                where userKra.UserId == UserId && kraLibrary.IsActive == (int)Status.IS_ACTIVE && (
                (userKra.IsActive == (int)Status.IN_ACTIVE && userKra.IsDeleted == (int)Status.IS_ACTIVE) ||
                (userKra.IsActive == (int)Status.IS_ACTIVE && (userKra.IsDeleted == null || userKra.IsDeleted == 0))
)
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
        public async Task<ResponseModel> AssignUnassignKra(int userKraId, byte IsActive, byte IsDeleted) // Is Deleted flag to unassign the kras so that the It can be assigned again.
        {
            ResponseModel model = new ResponseModel();
            UserKRA userKra = null;
            try
            {
                userKra = _dbcontext.UserKRA.Where(c => c.Id == userKraId).FirstOrDefault();
                if (userKra != null)
                {
                    userKra.IsActive = IsActive;
                    userKra.IsDeleted = IsDeleted;

                    _dbcontext.Update(userKra);
                    _dbcontext.SaveChanges();

                    model.IsSuccess = true;
                    if (userKra.IsActive == (int)Status.IN_ACTIVE && userKra.IsDeleted == (int)Status.IN_ACTIVE)
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }

        /// Get the list of Release kras for the resource quarterwise .
        public async Task<List<AssignedKras>> GetResourceReleasedKras(int quarterId, int userId)
        {
            try
            {
                var userKRAList = await (
                    from userKras in _dbcontext.UserKRA
                    join kraLibrary in _dbcontext.KRALibrary on userKras.KRAId equals kraLibrary.Id
                    where userKras.IsActive == (int)Status.IS_ACTIVE && kraLibrary.IsActive == (int)Status.IS_ACTIVE
                          && userKras.QuarterId == quarterId
                          && userKras.UserId == userId
                    select new AssignedKras
                    {
                        KRAName = kraLibrary.Name,
                        DisplayName = kraLibrary.DisplayName,
                        Description = kraLibrary.Description,
                        Weightage = kraLibrary.Weightage
                    }
                ).ToListAsync();

                return userKRAList;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }

    }
}

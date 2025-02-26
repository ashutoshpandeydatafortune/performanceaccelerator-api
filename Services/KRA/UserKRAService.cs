using System;
using System.Text;
using System.Linq;
using DF_PA_API.Models;
using System.Data.Entity;
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
            return await _dbcontext.UserKRA.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<UserKRA> GetUserKRAById(int userKRAId)
        {
            UserKRA userKRA;
            try
            {
                userKRA = await _dbcontext.UserKRA.Where(c => c.IsActive == (int)Status.IS_ACTIVE && c.Id == userKRAId).FirstOrDefaultAsync();
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
             * 2. Create/store notifications while finding user and reading template
             * 3. For each user => notifications map, send one single email pser user
             */

            ResponseModel model = new ResponseModel();

            bool created = await CreateKraEntries(userKRAModel);
            if (created)
            {
                Dictionary<int, UserNotificationData> notificationMap = await CreateNotifications(userKRAModel);

                // Create a list of tasks for sending emails
                var sendEmailTasks = notificationMap.Select(entry => SendNotification(entry.Value, Constant.KRA_CREATED_TEMPLATE_NAME)).ToList();

                // Wait for all email sending tasks to complete concurrently
                await Task.WhenAll(sendEmailTasks);

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
                    //To restrict the duplicate entries of kras for particular quarter and user. 'var kralist'
                    var kralist = _dbcontext.UserKRA.Where(kra =>
                     kra.QuarterId == item.QuarterId && kra.KRAId == item.KRAId && kra.UserId == item.UserId).ToList();

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
            catch
            {
                throw;
            }

            return true;
        }

        //public async Task<bool> CreateKraEntries(List<UserKRA> userKRAModel, string userAchievement, string managerQuartelyComment)
        //{
        //    try
        //    {
        //        if (userKRAModel == null || !userKRAModel.Any())
        //            return false; // No data to process

        //        var firstItem = userKRAModel.First(); // Get user and quarter details
        //        int userId = firstItem.UserId.Value;
        //        int quarterId = firstItem.QuarterId.Value;
        //        int createdBy = firstItem.CreateBy.Value;

        //        foreach (var item in userKRAModel)
        //        {
        //            // Check for existing KRA entries for the same quarter and user
        //            var kralist = _dbcontext.UserKRA.Where(kra =>
        //                kra.QuarterId == item.QuarterId && kra.KRAId == item.KRAId && kra.UserId == item.UserId).ToList();

        //            if (!kralist.Any())
        //            {
        //                item.ManagerComment = string.IsNullOrEmpty(item.DeveloperComment) ? "" : item.DeveloperComment;
        //                item.DeveloperComment = string.IsNullOrEmpty(item.DeveloperComment) ? "" : item.DeveloperComment;
        //                item.ApprovedBy = item.ApprovedBy ?? null;
        //                item.RejectedBy = item.RejectedBy ?? null;
        //                item.IsApproved = 0;
        //                item.IsActive = (int)Status.IS_ACTIVE;
        //                item.CreateBy = item.CreateBy;
        //                item.CreateDate = DateTime.Now;
        //                item.UpdateDate = DateTime.Now;

        //                await _dbcontext.AddAsync(item);
        //            }
        //            else
        //            {
        //                return false; // Prevent duplicate KRA entries
        //            }
        //        }

        //        // Check if the user already has a comment entry for this quarter
        //        var existingComment = _dbcontext.UserQuarterlyAchievements
        //            .FirstOrDefault(c => c.UserId == userId && c.QuarterId == quarterId);

        //        if (existingComment == null)
        //        {
        //            if (!string.IsNullOrEmpty(userAchievement) && !string.IsNullOrEmpty(managerQuartelyComment))
        //            {
        //                var newAchievementComment = new UserQuarterlyAchievement
        //                {
        //                    UserId = userId,
        //                    QuarterId = quarterId,
        //                    UserAchievement = userAchievement,
        //                    ManagerQuartelyComment = managerQuartelyComment,
        //                    CreateBy = createdBy,
        //                    CreateDate = DateTime.Now
        //                };
        //                await _dbcontext.UserQuarterlyAchievements.AddAsync(newAchievementComment);
        //            }


        //        }


        //        await _dbcontext.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        throw;
        //    }

        //    return true;
        //}



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
                notificationMap[userKRA.UserId.Value].Name = user.ResourceName;
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

            var subject = "";
            var headerContent = "";
            string emailContent = string.Empty;
            //Here mail is send on the basis of Kra updated and created.
            foreach (Notification notification in userNotificationData.Notifications)
            {
                if (notification.Title == Constant.SUBJECT_KRA_UPDATED)
                {
                    subject = Constant.SUBJECT_KRA_UPDATED;
                    headerContent = _fileUtil.GetTemplateContent(Constant.KRA_HEADER_REJECT_TEMPLATE_NAME);
                }
                else
                {
                    subject = Constant.SUBJECT_KRA_CREATED;
                    headerContent = _fileUtil.GetTemplateContent(Constant.KRA_HEADER_TEMPLATE_NAME);
                }
            }

            headerContent = headerContent.Replace("{NAME}", userNotificationData.Name);

            emailContent += headerContent;

            var bodyContent = _fileUtil.GetTemplateContent(templateName);

            StringBuilder builder = new StringBuilder();

            foreach (var notification in userNotificationData.Notifications)
            {
                builder.Append("<li>" + notification.Description + "</li>");
            }

            var KRAName = bodyContent.Replace("{KRA_NAMES}", builder.ToString());

            emailContent += KRAName;

            var footerContent = _fileUtil.GetTemplateContent(Constant.KRA_FOOTER_TEMPLATE_NAME);
            footerContent = footerContent.Replace("{CREATE_DATE}", DateTime.Now.ToString());

            emailContent += footerContent;

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

                // Create a list of tasks for sending emails
                var sendEmailTasks = notificationMap.Select(entry => SendNotification(entry.Value, Constant.KRA_CREATED_TEMPLATE_NAME)).ToList();

                // Wait for all email sending tasks to complete concurrently
                await Task.WhenAll(sendEmailTasks);

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
                    var existingComment =  _dbcontext.UserQuarterlyAchievements
                        .FirstOrDefault(c => c.UserId == userId && c.QuarterId == quarterId);

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
                // Log the exception properly (you can add logging here)
                throw;
            }
        }


        private async Task<Dictionary<int, UserNotificationData>> PrepareUpdateNotifications(List<UserKRA> userKRAModel)
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
                    Title = Constant.SUBJECT_KRA_UPDATED,
                    Description = userKRA.KRAName,     // store kra name
                    IsRead = 0,
                    IsActive = (int)Status.IS_ACTIVE,
                    CreateAt = DateTime.Now
                };

                if (userKRA.isUpdated == true)
                {
                    //Fetching the manager details.                    
                    var reportingTos = _dbcontext.Resources.Where(resources => resources.ResourceId == userKRA.UserId.Value).FirstOrDefault();
                    var managerDetails = _dbcontext.Resources.Where(resources => resources.ResourceId == reportingTos.ReportingTo.Value).FirstOrDefault();

                    //Sending mail according to developer and manager action.
                    if ((userKRA.ManagerRating == null || userKRA.ManagerRating == 0)
                        && (userKRA.RejectedBy == null || userKRA.RejectedBy == 0))
                    {
                        notificationMap[userKRA.UserId.Value].Email = managerDetails.EmailId;
                        notificationMap[userKRA.UserId.Value].Name = managerDetails.ResourceName;
                    }

                    else if (((userKRA.ManagerRating != null && userKRA.ManagerRating != 0) || (userKRA.FinalRating != 0))
                             && userKRA.DeveloperRating != null && userKRA.DeveloperRating != 0
                             && (userKRA.RejectedBy != 0) || userKRA.RejectedBy == 0)
                    {
                        notificationMap[userKRA.UserId.Value].Email = user.EmailId;
                        notificationMap[userKRA.UserId.Value].Name = user.ResourceName;
                    }

                    else if (userKRA.RejectedBy != null && userKRA.RejectedBy != 0
                             && (userKRA.ManagerRating == null || userKRA.ManagerRating == 0))
                    {
                        notificationMap[userKRA.UserId.Value].Email = user.EmailId;
                        notificationMap[userKRA.UserId.Value].Name = user.ResourceName;
                    }
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
            catch
            {
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
                    where userKra.UserId == UserId && kraLibrary.IsActive == (int)Status.IS_ACTIVE
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
            catch (Exception)
            {
                throw;
            }
        }

        //UnassignKra is used to removed the assigned kras from resources.
        public async Task<ResponseModel>AssignUnassignKra(int userKraId, byte IsActive)
        {
            ResponseModel model = new ResponseModel();
            UserKRA userKra = null;
            try
            {
                userKra =  _dbcontext.UserKRA.Where(c=>c.Id == userKraId).FirstOrDefault();
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
                model.Messsage = "Error : " + ex.Message;
            }

            return model;
        }
    }
}

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
using static DF_EvolutionAPI.Models.UserKRADetails;
using Azure;

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

        //Displaying all kra's according to designation Id.
        public List<UserAssignedKRA> GetAssignedKRAsByDesignationId(int designationId)
        {
            try
            {
                var query = from resource in _dbcontext.Resources
                            join userKRA in _dbcontext.UserKRA on resource.ResourceId equals userKRA.UserId
                            join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                            where resource.DesignationId == designationId && userKRA.IsActive == 1
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
                foreach (var entry in notificationMap)
                {
                    await SendNotification(entry.Value, Constant.KRA_CREATED_TEMPLATE_NAME);
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
                    //To restrict the duplicate entries of kras for particular quarter and user. 'var kralist'
                    var kralist = _dbcontext.UserKRA.Where(kra => 
                     kra.QuarterId == item.QuarterId && kra.KRAId == item.KRAId && kra.UserId == item.UserId).ToList();

                    if (kralist.Count == 0)
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
                    IsActive = 1,
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

        public async Task<ResponseModel> UpdateUserKra(List<UserKRA> userKRAModel)
        {
            /**
             * This method performs following actions:
             * 1. Run loop on passed kras and store in database
             * 2. Update/store notifications while finding user and reading template
             * 3. For each user => notifications map, send one single email pser user
             */
            ResponseModel model = new ResponseModel();

            bool created = await UpdateKraEntries(userKRAModel);
            if (created)
            {
                Dictionary<int, UserNotificationData> notificationMap = await CreateUpdateNotifications(userKRAModel);
                foreach (var entry in notificationMap)
                {
                    await SendNotification(entry.Value, Constant.KRA_CREATED_TEMPLATE_NAME);
                }

                model.IsSuccess = true;
            }
            else
            {
                model.IsSuccess = false;
            }
            return model;
        }

        public async Task<bool> UpdateKraEntries(List<UserKRA> userKRAModel)
        {
            try
            {
                foreach (var userKRAModels in userKRAModel)
                {
                    var userKra = await _dbcontext.UserKRA.FindAsync(userKRAModels.Id);
                    var weightage =  (from kraLibrary in _dbcontext.KRALibrary
                                           where kraLibrary.Id == userKra.KRAId
                                           select kraLibrary.Weightage).FirstOrDefault();
                    if (userKra != null)
                    {
                        userKra.Reason = userKRAModels.Reason ?? null;
                        userKra.Comment = userKRAModels.Comment ?? null;
                        userKra.ApprovedBy = userKRAModels.ApprovedBy ?? null;
                        userKra.RejectedBy = userKRAModels.RejectedBy ?? null;
                        userKra.FinalComment = userKRAModels.FinalComment ?? null;
                        userKra.ManagerComment = userKRAModels.ManagerComment ?? null;
                        userKra.FinalRating = userKRAModels.FinalRating ?? null;
                        userKra.DeveloperComment = userKRAModels.DeveloperComment;
                        userKra.ManagerRating = userKRAModels.ManagerRating ?? null;
                        userKra.AppraisalRange = userKRAModels.AppraisalRange ?? null;
                        userKra.DeveloperRating = userKRAModels.DeveloperRating ?? null;
                        
                       //if (userKRAModels.FinalComment != null && userKRAModels.FinalRating.HasValue && weightage != 0)
                       if (userKRAModels.ManagerRating != null && userKRAModels.FinalRating.HasValue && weightage != 0)
                        {
                            userKra.Score = (double)userKRAModels.FinalRating * (double)weightage;
                        }
                        else
                        {
                            userKra.Score = 0;
                        }

                        userKra.IsActive = userKRAModels.IsActive;
                        userKra.UpdateBy = 1;
                        userKra.UpdateDate = DateTime.Now;

                        await _dbcontext.SaveChangesAsync();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
            return true;
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
                    IsActive = 1,
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
            UserKRA userKra = null;
            try
            {
                userKra = _dbcontext.UserKRA.Where(c => c.IsActive == 1 && c.Id == userKRAId).FirstOrDefault();

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
                        Description = kraLibrary.Description

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
                    where (resources.ResourceId == userId && quarterDetail.QuarterYearRange == quarterYearRange && quarterDetail.IsActive == 1 && userKRA.IsActive == 1 && userKRA.FinalComment != null)
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
                    if (userKra.IsActive == 0)
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

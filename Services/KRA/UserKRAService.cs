using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRA;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DF_EvolutionAPI.Services
{
    public class UserKRAService : IUserKRAService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public UserKRAService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
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
            ResponseModel model = new ResponseModel();

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

                model.IsSuccess = true;
                model.Messsage = "User KRA Inserted Successfully";

            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }

            return model;
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

        public List<UserKRARatingList> GetUserKraGraph(int UserId, string QuarderYearRange)
        {
            try
            {
                //var isActiveCondition = UserId == 0 ? 0 : 1;
                var rating = (
                    from project in _dbcontext.Projects
                    join projectResource in _dbcontext.ProjectResources on project.ProjectId equals projectResource.ProjectId
                    join resources in _dbcontext.Resources on projectResource.ResourceId equals resources.ResourceId
                    join userKRA in _dbcontext.UserKRA on resources.ResourceId equals userKRA.UserId
                    join quarterDetail in _dbcontext.QuarterDetails on userKRA.QuarterId equals quarterDetail.Id
                    join kraLibrary in _dbcontext.KRALibrary on userKRA.KRAId equals kraLibrary.Id
                    join designation in _dbcontext.Designations on resources.DesignationId equals designation.DesignationId
                    where resources.ResourceId == UserId && quarterDetail.QuarterYearRange == QuarderYearRange
                    group new { kraLibrary, userKRA, quarterDetail } by new { quarterDetail.QuarterYear, quarterDetail.QuarterYearRange, quarterDetail.Id, quarterDetail.QuarterName,  } into grouped
                    select new
                    {
                        Id = grouped.Key.Id,
                        QuarterName = grouped.Key.QuarterName,
                        QuarterYear = grouped.Key.QuarterYear,
                        QuaterYearRange = grouped.Key.QuarterYearRange,
                        Weightage = grouped.Sum(x => x.kraLibrary.Weightage),
                        Score = grouped.Sum(x => x.userKRA.FinalRating * x.kraLibrary.Weightage)

                    }
                    ).ToList();

                var result = rating.Select(r => new UserKRARatingList
                {
                   // QuarterYear = r.QuarterYear,
                    QuarterYearRange = r.QuaterYearRange,
                    QuarterName = r.QuarterName,                    
                    Rating = Math.Round((double)r.Score / (double)r.Weightage, 2)
                })
                    .OrderByDescending(x=>x.QuarterYearRange)
                    .ToList();

                return result;


            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

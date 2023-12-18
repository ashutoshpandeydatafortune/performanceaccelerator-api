using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRA;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                                KRAId = userKRA.KRAId,
                                KRAName = kraLibrary.Name,
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
                        userKra.Reason = string.IsNullOrEmpty(userKRAModel.Reason) ? userKra.Reason : userKRAModel.Reason;
                        userKra.Comment = string.IsNullOrEmpty(userKRAModel.Comment) ? userKra.Comment : userKRAModel.Comment;
                        userKra.FinalComment = string.IsNullOrEmpty(userKRAModel.FinalComment) ? " " : userKRAModel.FinalComment;
                        userKra.ManagerComment = string.IsNullOrEmpty(userKRAModel.ManagerComment) ? userKra.ManagerComment : userKRAModel.ManagerComment;
                        userKra.DeveloperComment = string.IsNullOrEmpty(userKRAModel.DeveloperComment) ? userKra.DeveloperComment : userKRAModel.DeveloperComment;
                        userKra.ApprovedBy = userKRAModel.ApprovedBy == null ? userKra.ApprovedBy : userKRAModel.ApprovedBy;
                        userKra.RejectedBy = userKRAModel.RejectedBy == null ? userKra.RejectedBy : userKRAModel.RejectedBy;

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
                    }
                    else
                    {
                        model.Messsage = "User KRA not found for update.";
                        model.IsSuccess = false;
                        return model; 
                    }

                   await _dbcontext.SaveChangesAsync();
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
                    from a in _dbcontext.KRALibrary
                    join c in _dbcontext.UserKRA on a.Id equals c.KRAId
                    join q in _dbcontext.QuarterDetails on c.QuarterId equals q.Id
                    //join S in _dbcontext.StatusLibrary on c.Status equals S.Id
                    where c.UserId == UserId
                    select new UserKRADetails
                    {
                        Id = c.Id,
                        KRAName = a.Name,
                        KRADisplayName = a.DisplayName,
                        UserId = c.UserId,
                        KRAId = c.KRAId,
                        QuarterId = (int)c.QuarterId,
                        QuarterName = q.QuarterName,
                        QuarterYear = q.QuarterYear,
                        Weightage = a.Weightage,
                        WeightageId = a.WeightageId,
                        DeveloperComment = c.DeveloperComment,
                        DeveloperRating = (int)c.DeveloperRating,
                        ManagerComment = c.ManagerComment,
                        ManagerRating = (int)c.ManagerRating,
                        FinalRating = (int)c.FinalRating,
                        FinalComment = c.FinalComment,
                        Score = c.Score,
                        Status = c.Status,
                        //StatusName = S.StatusName,
                        Reason = c.Reason
                    }).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return userKRADetails;
        }
    }
}

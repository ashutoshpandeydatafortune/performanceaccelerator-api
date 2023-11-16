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
               userKRA = await _dbcontext.UserKRA.Where(c => c.IsActive == 1 && c.Id==userKRAId).FirstOrDefaultAsync();
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
                                UserId = resource.ResourceId,
                                UserName = resource.ResourceName,
                                IsSpecial = kraLibrary.IsSpecial
                            };

                return query.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseModel> CreateUserKRA(UserKRA userKRAModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                userKRAModel.QuarterId = userKRAModel.QuarterId;
                userKRAModel.UserId = userKRAModel.UserId;
                userKRAModel.KRAId = userKRAModel.KRAId;
                userKRAModel.IsActive = 1;
                userKRAModel.CreateBy = 1;
                userKRAModel.UpdateBy = 1;
                userKRAModel.CreateDate = DateTime.Now;
                userKRAModel.UpdateDate = DateTime.Now;
                
                await _dbcontext.AddAsync(userKRAModel);
                _dbcontext.SaveChanges();

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

        public async Task<ResponseModel> CreateorUpdateUserKRA(UserKRA userKRAModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                UserKRA userKra = await GetUserKRAById(userKRAModel.Id);

                if (userKra != null)
                {
                    userKra.DeveloperComment = string.IsNullOrEmpty(userKRAModel.DeveloperComment) ? " " : userKRAModel.DeveloperComment; ;
                    userKra.DeveloperRating = userKRAModel.DeveloperRating;
                    userKra.ManagerComment = string.IsNullOrEmpty(userKRAModel.ManagerComment) ? " " : userKRAModel.ManagerComment;
                    userKra.ManagerRating = userKRAModel.ManagerRating;
                    userKra.FinalRating = userKRAModel.FinalRating;
                    userKra.FinalComment = string.IsNullOrEmpty(userKRAModel.FinalComment) ? " " : userKRAModel.FinalComment;
                    userKra.KRAId = userKRAModel.KRAId;
                    userKra.QuarterId = userKRAModel.QuarterId;
                    userKra.Score = userKRAModel.Score;
                    userKra.Status = userKRAModel.Status;
                    userKra.UserId = userKRAModel.UserId;
                    userKra.ApprovedBy = userKRAModel.ApprovedBy;
                    userKra.RejectedBy = userKRAModel.RejectedBy;
                    userKra.AppraisalRange = userKRAModel.AppraisalRange;
                    userKra.Reason = userKRAModel.Reason;
                    userKra.Comment = userKRAModel.Comment;
                    userKra.IsActive = 1;
                    userKra.UpdateBy = 1;
                    userKra.UpdateDate = DateTime.Now;

                    _dbcontext.Update<UserKRA>(userKra);

                    model.Messsage = "User KRA Update Successfully";
                }
                else
                {
                    userKRAModel.DeveloperComment = string.IsNullOrEmpty(userKRAModel.DeveloperComment) ? " " : userKRAModel.DeveloperComment;
                    userKRAModel.DeveloperRating = userKRAModel.DeveloperRating;
                    userKRAModel.ManagerComment = string.IsNullOrEmpty(userKRAModel.DeveloperComment) ? " " : userKRAModel.DeveloperComment;
                    userKRAModel.ManagerRating = userKRAModel.ManagerRating;
                    userKRAModel.FinalRating = userKRAModel.FinalRating;
                    userKRAModel.FinalComment = userKRAModel.FinalComment;
                    userKRAModel.QuarterId = userKRAModel.QuarterId;
                    userKRAModel.Score = userKRAModel.Score;
                    userKRAModel.Status = userKRAModel.Status;
                    userKRAModel.UserId = userKRAModel.UserId;
                    userKRAModel.KRAId = userKRAModel.KRAId;
                    userKRAModel.ApprovedBy = userKRAModel.ApprovedBy;
                    userKRAModel.RejectedBy = userKRAModel.RejectedBy;
                    userKRAModel.AppraisalRange = userKRAModel.AppraisalRange;
                    userKRAModel.Reason = userKRAModel.Reason;
                    userKRAModel.Comment = userKRAModel.Comment;
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

        public async Task<List<UserKRADetails>> GetKRAsByUserId(int? UserId)
        {
            var userKRADetails = new List<UserKRADetails>();

            try
            {
                userKRADetails = await (
                    from a in _dbcontext.KRALibrary
                    join c in _dbcontext.UserKRA on a.Id equals c.KRAId
                    join S in _dbcontext.StatusLibrary on c.Status equals S.Id
                    where c.UserId == UserId
                    select new UserKRADetails
                    {
                        Id = c.Id,
                        Name = a.Name,
                        UserId = c.UserId,
                        KRAId = c.KRAId,
                        QuarterId = (int)c.QuarterId,
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
                        StatusName = S.StatusName,
                        Reason = c.Reason
                    }).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return userKRADetails;
        }
    }
}

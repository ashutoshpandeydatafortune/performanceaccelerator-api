using DF_EvolutionAPI.Models;
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
               // userKRA = await _dbcontext.FindAsync<UserKRA>(userKRAId);
               userKRA=  _dbcontext.UserKRA.Where(c => c.IsActive == 1 && c.Id==userKRAId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
            return userKRA;
        }
        public async Task<ResponseModel> CreateUserKRA(UserKRA userKRAModel)
        {
            ResponseModel model = new ResponseModel();
            UserKRA _temp;
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
                _dbcontext.Add<UserKRA>(userKRAModel);
                model.Messsage = "User KRA Inserted Successfully";

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

        public async Task<ResponseModel> CreateorUpdateUserKRA(UserKRA userKRAModel)
        {
            ResponseModel model = new ResponseModel();
            
            try
            {
                UserKRA _temp = await GetUserKRAById(userKRAModel.Id);
                    if (_temp != null)
                    {
                        _temp.DeveloperComment = String.IsNullOrEmpty(userKRAModel.DeveloperComment) ? " " : userKRAModel.DeveloperComment; ;
                        _temp.DeveloperRating =  userKRAModel.DeveloperRating; 
                        _temp.ManagerComment =String.IsNullOrEmpty(userKRAModel.ManagerComment) ? " " : userKRAModel.ManagerComment; 
                        _temp.ManagerRating = userKRAModel.ManagerRating; 
                        _temp.FinalRating = userKRAModel.FinalRating; 
                        _temp.FinalComment = String.IsNullOrEmpty(userKRAModel.FinalComment) ? " " : userKRAModel.FinalComment;
                        _temp.KRAId = userKRAModel.KRAId;
                        _temp.QuarterId = userKRAModel.QuarterId;
                        _temp.Score = userKRAModel.Score; 
                        _temp.Status = userKRAModel.Status; 
                        _temp.UserId = userKRAModel.UserId;
                        _temp.ApprovedBy = userKRAModel.ApprovedBy;
                        _temp.RejectedBy = userKRAModel.RejectedBy;
                        _temp.AppraisalRange = userKRAModel.AppraisalRange;
                        _temp.Reason = userKRAModel.Reason;
                        _temp.Comment=userKRAModel.Comment;
                        _temp.IsActive = 1;
                        _temp.UpdateBy = 1;
                        _temp.UpdateDate = DateTime.Now;
                        _dbcontext.Update<UserKRA>(_temp);
                        model.Messsage = "User KRA Update Successfully";
                }
                else
                {
                    userKRAModel.DeveloperComment = String.IsNullOrEmpty(userKRAModel.DeveloperComment)?" ":userKRAModel.DeveloperComment;
                    userKRAModel.DeveloperRating = userKRAModel.DeveloperRating; 
                    userKRAModel.ManagerComment =String.IsNullOrEmpty(userKRAModel.DeveloperComment) ? " " : userKRAModel.DeveloperComment; 
                    userKRAModel.ManagerRating =userKRAModel.ManagerRating; 
                    userKRAModel.FinalRating =userKRAModel.FinalRating;
                    userKRAModel.FinalComment = userKRAModel.FinalComment;
                    userKRAModel.QuarterId = userKRAModel.QuarterId;
                    userKRAModel.Score =userKRAModel.Score; 
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
                    _dbcontext.Add<UserKRA>(userKRAModel);
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
                UserKRA _temp = await GetUserKRAById(userKRAId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<UserKRA>(_temp);
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
          var USerKRADetails=new List<UserKRADetails>();
            try
            {
                //KRAList = await (from k in _dbcontext.KRALibrary.Where(x => x.IsActive == 1)
                //                 let userKRA = (_dbcontext.UserKRA.Where(u => (int?)u.UserId == UserId && u.IsActive == 1)).ToList()
                //                 select new KRALibrary
                //                 {
                //                     Id=k.Id,
                //                     Name=k.Name
                //                 }).ToListAsync();


                USerKRADetails = await (from a in _dbcontext.KRALibrary
                               join c in _dbcontext.UserKRA on a.Id equals c.KRAId
                               join S in _dbcontext.StatusLibrary on c.Status equals S.Id
                               where c.UserId == UserId
                              select new UserKRADetails
                              {
                                  Id=c.Id,
                                  Name=a.Name,
                                  UserId=c.UserId,
                                  KRAId=c.KRAId,
                                  QuarterId = (int)c.QuarterId,
                                  Weightage=a.Weightage,
                                  WeightageId=a.WeightageId,   
                                  DeveloperComment = c.DeveloperComment,
                                  DeveloperRating= (int)c.DeveloperRating,    
                                  ManagerComment=c.ManagerComment,
                                  ManagerRating= (int)c.ManagerRating,
                                  FinalRating= (int)c.FinalRating,
                                  FinalComment=c.FinalComment,
                                  Score=c.Score,
                                  Status=c.Status,
                                  StatusName=S.StatusName,
                                  Reason=c.Reason

                                  //developerRating=c.DeveloperRating,
                              })
              .ToListAsync();
            }
            catch (Exception ex)
            {

            }
            return USerKRADetails;
        }
    }
}

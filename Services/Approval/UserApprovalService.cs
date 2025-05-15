using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DF_PA_API.Services;

namespace DF_EvolutionAPI.Services
{
    public class UserApprovalService : IUserApprovalService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<UserApprovalService> _logger;

        public UserApprovalService(DFEvolutionDBContext dbContext, ILogger<UserApprovalService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
        }

        public async Task<List<UserApproval>> GetAllApprovalList()
        {
            return await _dbcontext.UserApproval.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
        }

        public async Task<UserApproval> GetApprovalById(int userApprovalId)
        {
            UserApproval userApproval;
            
            try
            {
                userApproval = await _dbcontext.FindAsync<UserApproval>(userApprovalId);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        
            return userApproval;
        }
        public async Task<ResponseModel> CreateorUpdateApproval(UserApproval userApprovalModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                UserApproval userApproval = await GetApprovalById(userApprovalModel.Id);

                if (userApproval != null)
                {
                    userApproval.Comment = userApprovalModel.Comment;
                    userApproval.ApprovalStatus = userApprovalModel.ApprovalStatus;
                    userApproval.ApprovedBy = userApprovalModel.ApprovedBy;
                    userApproval.Reason = userApprovalModel.Reason;
                    userApproval.RejectedBy = userApprovalModel.RejectedBy;
                    userApproval.AppraisalRange = userApprovalModel.AppraisalRange;
                    userApproval.IsActive = (int)Status.IS_ACTIVE;
                    userApproval.UpdateBy = (int)Status.IS_ACTIVE;
                    userApproval.UpdateDate = DateTime.Now;

                    _dbcontext.Update(userApproval);
                    
                    model.Messsage = "User Approval Update Successfully";
                }
                else
                {
                    userApprovalModel.UserId = userApprovalModel.UserId;
                    userApprovalModel.KRAId=userApprovalModel.KRAId;
                    userApprovalModel.IsActive = (int)Status.IS_ACTIVE;
                    userApprovalModel.CreateBy = (int)Status.IS_ACTIVE;
                    userApprovalModel.UpdateBy = (int)Status.IS_ACTIVE;
                    userApprovalModel.CreateDate = DateTime.Now;
                    userApprovalModel.UpdateDate = DateTime.Now;

                    _dbcontext.Add(userApprovalModel);
                    
                    model.Messsage = "User Approval Inserted Successfully";
                }

                _dbcontext.SaveChanges();
                
                model.IsSuccess = true;
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }
        public async Task<ResponseModel> DeleteApproval(int userApprovalId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                UserApproval userApproval = await GetApprovalById(userApprovalId);
            
                if (userApproval != null)
                {
                    userApproval.IsDeleted = 1;

                    _dbcontext.Update(userApproval);
                    _dbcontext.SaveChanges();
                    
                    model.IsSuccess = true;
                    model.Messsage = "User ApprovalDeleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "User Approval Not Found";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }
    }
}

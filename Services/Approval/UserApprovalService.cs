using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class UserApprovalService : IUserApprovalService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        public UserApprovalService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<List<UserApproval>> GetAllApprovalList()
        {
            return await _dbcontext.UserApproval.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<UserApproval> GetApprovalById(int userApprovalId)
        {
            UserApproval userApproval;
            try
            {
                userApproval = await _dbcontext.FindAsync<UserApproval>(userApprovalId);
            }
            catch (Exception)
            {
                throw;
            }
            return userApproval;
        }
        public async Task<ResponseModel> CreateorUpdateApproval(UserApproval userApprovalModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                UserApproval _temp = await GetApprovalById(userApprovalModel.Id);
                if (_temp != null)
                {
                    _temp.Comment = userApprovalModel.Comment;
                    _temp.ApprovalStatus = userApprovalModel.ApprovalStatus;
                    _temp.ApprovedBy = userApprovalModel.ApprovedBy;
                    _temp.Reason = userApprovalModel.Reason;
                    _temp.RejectedBy = userApprovalModel.RejectedBy;
                    _temp.AppraisalRange = userApprovalModel.AppraisalRange;
                    _temp.IsActive = 1;
                    _temp.UpdateBy = 1;
                    _temp.UpdateDate = DateTime.Now;
                    _dbcontext.Update<UserApproval>(_temp);
                    model.Messsage = "User Approval Update Successfully";
                }
                else
                {
                    userApprovalModel.UserId = userApprovalModel.UserId;
                    userApprovalModel.KRAId=userApprovalModel.KRAId;
                    userApprovalModel.IsActive = 1;
                    userApprovalModel.CreateBy = 1;
                    userApprovalModel.UpdateBy = 1;
                    userApprovalModel.CreateDate = DateTime.Now;
                    userApprovalModel.UpdateDate = DateTime.Now;
                    _dbcontext.Add<UserApproval>(userApprovalModel);
                    model.Messsage = "User Approval Inserted Successfully";
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
        public async Task<ResponseModel> DeleteApproval(int userApprovalId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                UserApproval _temp = await GetApprovalById(userApprovalId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<UserApproval>(_temp);
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
                model.Messsage = "Error : " + ex.Message;
            }
            return model;
        }
    }
}

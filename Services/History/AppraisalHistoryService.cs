using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.History
{
    public class AppraisalHistoryService : IAppraisalHistoryService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        public AppraisalHistoryService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<List<AppraisalHistory>> GetAllAppraisalHistoryList()
        {
            return await _dbcontext.AppraisalHistory.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<AppraisalHistory> GetAppraisalHistoryById(int appraisalHistoryId)
        {
            AppraisalHistory userApproval;
            try
            {
                userApproval = await _dbcontext.FindAsync<AppraisalHistory>(appraisalHistoryId);
            }
            catch (Exception)
            {
                throw;
            }
            return userApproval;
        }
        public async Task<ResponseModel> CreateorUpdateAppraisalHistory(AppraisalHistory appraisalHistoryModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                AppraisalHistory _temp = await GetAppraisalHistoryById(appraisalHistoryModel.Id);
                if (_temp != null)
                {
                    _temp.UserId = appraisalHistoryModel.UserId;
                    _temp.LastAppraisal = appraisalHistoryModel.LastAppraisal;
                    _temp.Percentage = appraisalHistoryModel.Percentage;
                    _temp.LastAppraisalDate = appraisalHistoryModel.LastAppraisalDate;
                    _temp.IsActive = 1;
                    _temp.UpdateBy = 1;
                    _temp.UpdateDate = DateTime.Now;
                    _dbcontext.Update<AppraisalHistory>(_temp);
                    model.Messsage = "Appraisal History Update Successfully";
                }
                else
                {
                    appraisalHistoryModel.IsActive = 1;
                    appraisalHistoryModel.CreateBy = 1;
                    appraisalHistoryModel.UpdateBy = 1;
                    appraisalHistoryModel.CreateDate = DateTime.Now;
                    appraisalHistoryModel.UpdateDate = DateTime.Now;
                    _dbcontext.Add<AppraisalHistory>(appraisalHistoryModel);
                    model.Messsage = "Appraisal History Inserted Successfully";
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

        public async Task<ResponseModel> DeleteAppraisalHistory(int appraisalHistoryId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                AppraisalHistory _temp = await GetAppraisalHistoryById(appraisalHistoryId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<AppraisalHistory>(_temp);
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


        public async Task<ResponseModel> GetAppraisalHistoryByUserId(int userId)
        {
            ResponseModel model = new ResponseModel();
            var appraisalHistoryDetails = new List<AppraisalHistory>();
            try
            {
                appraisalHistoryDetails = await (from ah in _dbcontext.AppraisalHistory.Where(x => x.UserId == userId)
                                                 select new AppraisalHistory
                                                 {
                                                     CreateDate = ah.CreateDate,
                                                     UpdateDate = ah.UpdateDate,

                                                 }).ToListAsync();
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }
            return model;
        }

        public async Task<ResponseModel> GetUserDetailsByAppraisalHistoryId(int appraisalHistoryId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                AppraisalHistory appraisalHistory = await GetAppraisalHistoryById(appraisalHistoryId);
                var userInfo = await (from s in _dbcontext.UserKRA
                                      where s.Id == appraisalHistory.UserId && s.IsActive == 1
                                      select s).FirstAsync();
                model.IsSuccess = true;
                if (userInfo == null)
                {
                    model.Messsage = "User Details Not Found";
                    return model;
                }

                model.Messsage = "User Details Found Successfully";
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

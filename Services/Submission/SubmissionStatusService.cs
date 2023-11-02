using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Submission
{
    public class SubmissionStatusService : ISubmissionStatusService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        public SubmissionStatusService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<SubmissionStatus>> GetAllSubmissionStatusList()
        {
            return await _dbcontext.SubmissionStatus.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<SubmissionStatus> GetSubmissionStatusById(int submissionStatusId)
        {
            SubmissionStatus submissionStatus;
            try
            {
                submissionStatus = await _dbcontext.FindAsync<SubmissionStatus>(submissionStatusId);
            }
            catch (Exception)
            {
                throw;
            }
            return submissionStatus;
        }
        public async Task<ResponseModel> CreateorUpdateSubmissionStatus(SubmissionStatus submissionstatusModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                SubmissionStatus _temp = await GetSubmissionStatusById(submissionstatusModel.Id);
                if (_temp != null)
                {
                    _temp.SubmissionName = submissionstatusModel.SubmissionName;
                    _temp.Description = submissionstatusModel.Description;
                    _temp.StatusId = submissionstatusModel.StatusId;
                    _temp.IsActive = 1;
                    _temp.UpdateBy = 1;
                    _temp.UpdateDate = DateTime.Now;
                    _dbcontext.Update<SubmissionStatus>(_temp);
                    model.Messsage = "Submission Status Update Successfully";
                }
                else
                {
                    submissionstatusModel.IsActive = 1;
                    submissionstatusModel.CreateBy = 1;
                    submissionstatusModel.UpdateBy = 1;
                    submissionstatusModel.CreateDate = DateTime.Now;
                    submissionstatusModel.UpdateDate = DateTime.Now;
                    _dbcontext.Add<SubmissionStatus>(submissionstatusModel);
                    model.Messsage = "Submission Status Inserted Successfully";
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

        public async Task<ResponseModel> DeleteSubmissionStatus(int submissionStatusId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                SubmissionStatus _temp = await GetSubmissionStatusById(submissionStatusId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<SubmissionStatus>(_temp);
                    _dbcontext.SaveChanges();
                    model.IsSuccess = true;
                    model.Messsage = "Submission Status Deleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Submission Status Not Found";
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

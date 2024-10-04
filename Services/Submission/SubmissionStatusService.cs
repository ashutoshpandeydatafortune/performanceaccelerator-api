using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using DF_PA_API.Models;
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
            return await _dbcontext.SubmissionStatus.Where(c => c.IsActive == (int)Status.IS_ACTIVE).ToListAsync();
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
                SubmissionStatus submissionStatus = await GetSubmissionStatusById(submissionstatusModel.Id);
            
                if (submissionStatus != null)
                {
                    submissionStatus.SubmissionName = submissionstatusModel.SubmissionName;
                    submissionStatus.Description = submissionstatusModel.Description;
                    submissionStatus.StatusId = submissionstatusModel.StatusId;
                    submissionStatus.IsActive = (int)Status.IS_ACTIVE;
                    submissionStatus.UpdateBy = 1;
                    submissionStatus.UpdateDate = DateTime.Now;

                    _dbcontext.Update(submissionStatus);
                    
                    model.Messsage = "Submission Status Update Successfully";
                }
                else
                {
                    submissionstatusModel.IsActive = (int)Status.IS_ACTIVE;
                    submissionstatusModel.CreateBy = 1;
                    submissionstatusModel.UpdateBy = 1;
                    submissionstatusModel.CreateDate = DateTime.Now;
                    submissionstatusModel.UpdateDate = DateTime.Now;

                    _dbcontext.Add(submissionstatusModel);
                    
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
                SubmissionStatus submissionStatus = await GetSubmissionStatusById(submissionStatusId);

                if (submissionStatus != null)
                {
                    submissionStatus.IsDeleted = 1;

                    _dbcontext.Update(submissionStatus);
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

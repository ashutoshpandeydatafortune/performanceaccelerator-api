using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class StatusService : IStatusService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public StatusService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<StatusLibrary>> GetAllStatusList()
        {
            return await _dbcontext.StatusLibrary.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<StatusLibrary> GetStatusById(int statusId)
        {
            StatusLibrary statuslibrary;

            try
            {
                statuslibrary = await _dbcontext.FindAsync<StatusLibrary>(statusId);
            }
            catch (Exception)
            {
                throw;
            }
            
            return statuslibrary;
        }

        public async Task<ResponseModel> CreateorUpdateStatus(StatusLibrary statusModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                StatusLibrary statusLibrary = await GetStatusById(statusModel.Id);

                if (statusLibrary != null)
                {
                    statusLibrary.Id = statusModel.Id;
                    statusLibrary.StatusName = statusModel.StatusName;
                    statusLibrary.StatusType = statusModel.StatusType;
                    statusLibrary.Description = statusModel.Description;
                    statusLibrary.IsActive = 1;
                    statusLibrary.UpdateBy = 1;
                    statusLibrary.UpdateDate = DateTime.Now;
                    
                    _dbcontext.Update(statusLibrary);
                    
                    model.Messsage = "Status Updated Successfully";
                }
                else
                {
                    statusModel.IsActive = 1;
                    statusModel.CreateBy = 1;
                    statusModel.UpdateBy = 1;
                    statusModel.CreateDate = DateTime.Now;
                    statusModel.UpdateDate = DateTime.Now;

                    _dbcontext.Add(statusModel);
                    
                    model.Messsage = "Status Inserted Successfully";
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

        public async Task<ResponseModel> DeleteStatus(int statusId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                StatusLibrary statusLibrary = await GetStatusById(statusId);

                if (statusLibrary != null)
                {
                    statusLibrary.IsDeleted = 1;

                    _dbcontext.Update(statusLibrary);
                    _dbcontext.SaveChanges();

                    model.IsSuccess = true;
                    model.Messsage = "Status Deleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Status Not Found";
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

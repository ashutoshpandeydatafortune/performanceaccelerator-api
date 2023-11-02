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
                StatusLibrary _temp = await GetStatusById(statusModel.Id);
                if (_temp != null)
                {
                    _temp.Id = statusModel.Id;
                    _temp.StatusName = statusModel.StatusName;
                    _temp.StatusType = statusModel.StatusType;
                    _temp.Description = statusModel.Description;
                    _temp.IsActive = 1;
                    _temp.UpdateBy = 1;
                    _temp.UpdateDate = DateTime.Now;
                    _dbcontext.Update<StatusLibrary>(_temp);
                    model.Messsage = "Status Updated Successfully";
                }
                else
                {
                    statusModel.IsActive = 1;
                    statusModel.CreateBy = 1;
                    statusModel.UpdateBy = 1;
                    statusModel.CreateDate = DateTime.Now;
                    statusModel.UpdateDate = DateTime.Now;
                    _dbcontext.Add<StatusLibrary>(statusModel);
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
                StatusLibrary _temp = await GetStatusById(statusId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<StatusLibrary>(_temp);
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

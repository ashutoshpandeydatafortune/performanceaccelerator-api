using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class QuarterService : IQuarterService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public QuarterService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }
        public async Task<List<QuarterDetails>> GetAllQuarterList()
        {
            var quarterdetails= await _dbcontext.QuarterDetails.Where(c => c.IsActive == 1).ToListAsync();
            return quarterdetails;
        }

        public async Task<QuarterDetails> GetQuarterDetailsById(int quarterId)
        {
            QuarterDetails quarterdetails;
            try
            {
                quarterdetails = await _dbcontext.FindAsync<QuarterDetails>(quarterId);
            }
            catch (Exception) 
            {
                throw;
            }
            return quarterdetails;
        }

        public async Task<ResponseModel> CreateorUpdateQuarter(QuarterDetails quarterModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                QuarterDetails _temp = await GetQuarterDetailsById(quarterModel.Id);
                if (_temp != null)
                {
                    _temp.QuarterName = quarterModel.QuarterName;
                    _temp.QuarterYear = quarterModel.QuarterYear;
                    _temp.StatusId = quarterModel.StatusId;
                    _temp.Description = quarterModel.Description;
                    _temp.IsActive = 1;
                    _temp.UpdateBy = 1;
                    _temp.UpdateDate = DateTime.Now;
                    _dbcontext.Update<QuarterDetails>(_temp);
                    model.Messsage = "Quarter Details Updated Successfully";
                }
                else
                {
                    quarterModel.IsActive = 1;
                    quarterModel.CreateBy = 1;
                    quarterModel.UpdateBy = 1;
                    quarterModel.CreateDate = DateTime.Now;
                    quarterModel.UpdateDate = DateTime.Now;
                    _dbcontext.Add<QuarterDetails>(quarterModel);
                    model.Messsage = "Quarter Details Inserted Successfully";
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

        public async Task<ResponseModel> DeleteQuarter(int quarterId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                QuarterDetails _temp = await GetQuarterDetailsById(quarterId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<QuarterDetails>(_temp);
                    _dbcontext.SaveChanges();
                    model.IsSuccess = true;
                    model.Messsage = "Quarter Details Deleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Quarter Details Not Found";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }
            return model;
        }

        public async Task<ResponseModel> GetQuarterByStatusId(int statusId)
        {
            ResponseModel model = new ResponseModel();
            var quarterDetails = new List<QuarterDetails>();
            try
            {
                quarterDetails = await(from qd in _dbcontext.QuarterDetails.Where(x => x.StatusId == statusId)
                                         select new QuarterDetails
                                         {
                                             Id = qd.Id,
                                             Description = qd.Description,
                                             QuarterName = qd.QuarterName,
                                             QuarterYear = qd.QuarterYear,
                                             IsDeleted = qd.IsDeleted,
                                             IsActive = qd.IsActive,
                                             StatusId = qd.StatusId,
                                             CreateBy = qd.CreateBy,
                                             UpdateBy = qd.UpdateBy,
                                             CreateDate = qd.CreateDate,
                                             UpdateDate = qd.UpdateDate,
                                             
                                         }).ToListAsync();
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }
            return model;
        }

        public async Task<ResponseModel> GetStatusDetailsByQuarterId(int quarterId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                QuarterDetails quarterDetails = await GetQuarterDetailsById(quarterId);
                var status = await (from s in _dbcontext.KRAWeightages
                                          where s.Id == quarterDetails.StatusId && s.IsActive == 1
                                          select s).FirstAsync();
                model.IsSuccess = true;
                if (status == null)
                {

                    model.Messsage = "Status Not Found";
                    return model;
                }

                model.Messsage = "Status Found Successfully";
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

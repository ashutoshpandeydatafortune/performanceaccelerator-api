﻿using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using DF_EvolutionAPI.Utils;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Services.KRATemplate;

namespace DF_EvolutionAPI.Services
{
    public class QuarterService : IQuarterService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<QuarterService> _logger;

        public QuarterService(DFEvolutionDBContext dbContext, ILogger<QuarterService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
        }

        public async Task<List<QuarterDetails>> GetAllQuarterList(string type)
        {
            if (type == "current")
            {
                return await _dbcontext.QuarterDetails.Where(c => c.IsActive == (int)Status.IS_ACTIVE).OrderBy(x => x.QuarterYear).ToListAsync();
            }
            else
            {
                return await _dbcontext.QuarterDetails.OrderBy(x => x.QuarterYear).ToListAsync();
            }
        }

        public async Task<QuarterDetails> GetQuarterDetailsById(int quarterId)
        {
            QuarterDetails quarterdetails;
            
            try
            {
                quarterdetails = await _dbcontext.FindAsync<QuarterDetails>(quarterId);
            }
            catch (Exception ex) 
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }

            return quarterdetails;
        }

        public async Task<ResponseModel> CreateorUpdateQuarter(QuarterDetails quarterModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                QuarterDetails quarterDetails = await GetQuarterDetailsById(quarterModel.Id);

                if (quarterDetails != null)
                {
                    quarterDetails.QuarterName = quarterModel.QuarterName;
                    quarterDetails.QuarterYear = quarterModel.QuarterYear;
                    quarterDetails.QuarterYearRange = quarterModel.QuarterYearRange;
                    quarterDetails.StatusId = quarterModel.StatusId;
                    quarterDetails.Description = quarterModel.Description;
                    quarterDetails.IsActive = (int)Status.IS_ACTIVE;
                    quarterDetails.UpdateBy = 1;
                    quarterDetails.UpdateDate = DateTime.Now;
                    
                    _dbcontext.Update(quarterDetails);
                    
                    model.Messsage = "Quarter Details Updated Successfully";
                }
                else
                {
                    quarterModel.IsActive = (int)Status.IS_ACTIVE;
                    quarterModel.CreateBy = 1;
                    quarterModel.UpdateBy = 1;
                    quarterModel.CreateDate = DateTime.Now;
                    quarterModel.UpdateDate = DateTime.Now;
                    
                   
                    
                    _dbcontext.Add(quarterModel);
                    
                    model.Messsage = "Quarter Details Inserted Successfully";
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

        public async Task<ResponseModel> DeleteQuarter(int quarterId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                QuarterDetails quarterDetails = await GetQuarterDetailsById(quarterId);

                if (quarterDetails != null)
                {
                    quarterDetails.IsDeleted = 1;
                    _dbcontext.Update(quarterDetails);

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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }

        public async Task<ResponseModel> GetQuarterByStatusId(int statusId)
        {
            ResponseModel model = new ResponseModel();
            var quarterDetails = new List<QuarterDetails>();

            try
            {
                quarterDetails = await (
                    from qd in _dbcontext.QuarterDetails.Where(x => x.StatusId == statusId)
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
                        QuarterYearRange = qd.QuarterYearRange
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }

        public async Task<ResponseModel> GetStatusDetailsByQuarterId(int quarterId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                QuarterDetails quarterDetails = await GetQuarterDetailsById(quarterId);

                var status = (
                    from s in _dbcontext.KRAWeightages
                    where s.Id == quarterDetails.StatusId && s.IsActive == (int)Status.IS_ACTIVE
                    select s).First();

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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }
    }
}

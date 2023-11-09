using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DF_EvolutionAPI.Services
{
    public class QuarterService : IQuarterService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public QuarterService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public List<QuarterDetails> GetAllQuarterList()
        {
            return _dbcontext.QuarterDetails.Where(c => c.IsActive == 1).ToList();
        }

        public QuarterDetails GetQuarterDetailsById(int quarterId)
        {
            QuarterDetails quarterdetails;
            
            try
            {
                quarterdetails = _dbcontext.Find<QuarterDetails>(quarterId);
            }
            catch (Exception) 
            {
                throw;
            }

            return quarterdetails;
        }

        public ResponseModel CreateorUpdateQuarter(QuarterDetails quarterModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                QuarterDetails quarterDetails = GetQuarterDetailsById(quarterModel.Id);

                if (quarterDetails != null)
                {
                    quarterDetails.QuarterName = quarterModel.QuarterName;
                    quarterDetails.QuarterYear = quarterModel.QuarterYear;
                    quarterDetails.StatusId = quarterModel.StatusId;
                    quarterDetails.Description = quarterModel.Description;
                    quarterDetails.IsActive = 1;
                    quarterDetails.UpdateBy = 1;
                    quarterDetails.UpdateDate = DateTime.Now;
                    
                    _dbcontext.Update<QuarterDetails>(quarterDetails);
                    
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

        public ResponseModel DeleteQuarter(int quarterId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                QuarterDetails quarterDetails = GetQuarterDetailsById(quarterId);

                if (quarterDetails != null)
                {
                    quarterDetails.IsDeleted = 1;
                    _dbcontext.Update<QuarterDetails>(quarterDetails);

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

        public ResponseModel GetQuarterByStatusId(int statusId)
        {
            ResponseModel model = new ResponseModel();
            var quarterDetails = new List<QuarterDetails>();

            try
            {
                quarterDetails = (
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
                        UpdateDate = qd.UpdateDate                                             
                    }).ToList();
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }

            return model;
        }

        public ResponseModel GetStatusDetailsByQuarterId(int quarterId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                QuarterDetails quarterDetails = GetQuarterDetailsById(quarterId);

                var status = (
                    from s in _dbcontext.KRAWeightages
                    where s.Id == quarterDetails.StatusId && s.IsActive == 1
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
                model.Messsage = "Error : " + ex.Message;
            }

            return model;
        }

    }
}

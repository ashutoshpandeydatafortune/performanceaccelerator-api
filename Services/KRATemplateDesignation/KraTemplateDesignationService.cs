

using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.KRATemplateDesignation
{
    public class KraTemplateDesignationService: IKraTemplateDesignation
    {
        private readonly DFEvolutionDBContext _dbContext;

        public KraTemplateDesignationService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }
       

        public async Task<ResponseModel> AssignTemplateDesingation(PATemplateDesignation paTemplateDesignation)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                paTemplateDesignation.IsActive = true;
                paTemplateDesignation.CreateBy = 1;
                paTemplateDesignation.UpdateBy = 1;
                paTemplateDesignation.CreateDate = DateTime.Now;
                paTemplateDesignation.UpdateDate = DateTime.Now;

                _dbContext.Add(paTemplateDesignation);
                model.Messsage = "Template designation saved successfully.";

                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;
               
            }
            catch(Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = ex.Message;
            }
            return model;   
        }

        public async Task<ResponseModel> UnassignTemplateDesignation(int templateDesignationId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteTemplateDesignation = _dbContext.PA_TemplateDesignations.Find(templateDesignationId);
                if(deleteTemplateDesignation != null)
                {
                    deleteTemplateDesignation.IsActive = false;
                    deleteTemplateDesignation.CreateBy = 1;
                    deleteTemplateDesignation.UpdateBy = 1;
                    deleteTemplateDesignation.CreateDate = DateTime.Now;
                    deleteTemplateDesignation.UpdateDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                    model.Messsage = "Unsignned the template designation successfully.";

                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Assigned template does not found.";
                }

            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error" + ex.Message;

            }
            return model;
        }

    }
}

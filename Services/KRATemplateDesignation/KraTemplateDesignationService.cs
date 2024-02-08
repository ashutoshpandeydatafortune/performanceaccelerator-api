using System;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using DF_EvolutionAPI.Models.Response;

namespace DF_EvolutionAPI.Services.KRATemplateDesignation
{
    public class KraTemplateDesignationService: IKraTemplateDesignation
    {
        private readonly DFEvolutionDBContext _dbContext;

        public KraTemplateDesignationService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Assing the template to the designation by inserting record in PA_TemplateDesignation table.
        public async Task<ResponseModel> AssignTemplateDesingation(PATemplateDesignation paTemplateDesignation)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                paTemplateDesignation.CreateBy = 1;
                paTemplateDesignation.IsActive = 1;
                paTemplateDesignation.CreateDate = DateTime.Now;
              
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

        //Unassign the template to designation in PA_TemplateDesignation by making inactive record.
        public async Task<ResponseModel> UnassignTemplateDesignation(int templateDesignationId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteTemplateDesignation = _dbContext.PA_TemplateDesignations.Find(templateDesignationId);
                if(deleteTemplateDesignation != null)
                {
                    deleteTemplateDesignation.UpdateBy = 1;
                    deleteTemplateDesignation.IsActive = 0;
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

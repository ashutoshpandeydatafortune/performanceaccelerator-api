

using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System;
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
       

        public async Task<ResponseModel> CreateTemplateDesingation(PATemplateDesignation paTemplateDesignation)
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

    }
}

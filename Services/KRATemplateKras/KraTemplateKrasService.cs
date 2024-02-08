using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;
using System;

namespace DF_EvolutionAPI.Services.KRATemplateKras
{
    public class KraTemplateKrasService: IKraTemplateKras
    {
        private readonly DFEvolutionDBContext _dbContext;

        public KraTemplateKrasService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Assing the template to the designation by inserting record in PA_TemplateDesignation table.
        public async Task<ResponseModel> AssignTemplateKras(PATemplateKras paTemplateKras )
        {
            ResponseModel model = new ResponseModel();

            try
            {
                paTemplateKras.CreateBy = 1;
                paTemplateKras.IsActive = 1;
                paTemplateKras.CreateDate = DateTime.Now;
                
                _dbContext.Add(paTemplateKras);
                model.Messsage = "Template Kras saved successfully.";

                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;

            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = ex.Message;
            }
            return model;
        }

        //Unassign the template to designation in PA_TemplateDesignation by making inactive record.
        public async Task<ResponseModel> UnassignTemplateKras(int templateKrasId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteTemplateKras = _dbContext.PA_TemplateKras.Find(templateKrasId);
                if (deleteTemplateKras != null)
                {
                    deleteTemplateKras.UpdateBy = 1;
                    deleteTemplateKras.IsActive = 0;
                    deleteTemplateKras.UpdateDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                    model.Messsage = "Unsignned the template kras successfully.";
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

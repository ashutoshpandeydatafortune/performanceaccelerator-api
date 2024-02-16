using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using System.Threading.Tasks;
using System;
using System.Linq;

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
        public async Task<ResponseModel> AssignTemplateKras(PATtemplateKrasList paTemplateKras )
        {
            ResponseModel model = new ResponseModel();
            try
            {
               // var existingRecords = _dbContext.PA_TemplateKras.ToList();
                var existingRecords = _dbContext.PA_TemplateKras.Where(template => template.TemplateId == paTemplateKras.TemplateId).ToList();

                // Set IsActive to 0 for each existing record to mark them as inactive
                foreach (var existingRecord in existingRecords)
                {
                    existingRecord.IsActive = 0;
                    existingRecord.UpdateBy = 1;
                    existingRecord.UpdateDate = DateTime.Now;
                    _dbContext.PA_TemplateKras.Update(existingRecord);
                }
                // Assuming paTemplateKras.KraId is a collection of Kra IDs
                foreach (var kraid in paTemplateKras.KraIds)
                {
                    var newTemplateKras = new PATemplateKra
                    {
                        TemplateId = paTemplateKras.TemplateId,
                        KraId = kraid,
                        CreateBy = 1,
                        IsActive = 1,
                        CreateDate = DateTime.Now
                    };
                    _dbContext.PA_TemplateKras.Add(newTemplateKras);
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;
                model.Messsage = "Template Kras saved successfully.";
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = ex.Message;
            }
            return model;
        }    
    }
}

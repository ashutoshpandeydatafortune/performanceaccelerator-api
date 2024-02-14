using System;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace DF_EvolutionAPI.Services.KRATemplateDesignation
{
    public class KraTemplateDesignationService : IKraTemplateDesignation
    {
        private readonly DFEvolutionDBContext _dbContext;

        public KraTemplateDesignationService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel> AssignTemplateDesingation(PATtemplateDesignationList paTemplateDesignation)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var existingRecords = _dbContext.PA_TemplateDesignations.Where(template => template.TemplateId == paTemplateDesignation.TemplateId).ToList();

                // Set IsActive to 0 for all existing record to mark them as inactive
                foreach (var existingRecord in existingRecords)
                {
                    existingRecord.IsActive = 0;
                    existingRecord.UpdateBy = 1;
                    existingRecord.UpdateDate = DateTime.Now;
                    _dbContext.PA_TemplateDesignations.Update(existingRecord);
                }
                //Inserting the new reccord.
                foreach (var designationId in paTemplateDesignation.DesignationIds)
                {
                    var newDesignation = new PATemplateDesignation
                    {
                        TemplateId = paTemplateDesignation.TemplateId,
                        DesignationId = designationId,
                        IsActive = 1,
                        CreateBy = 1,
                        CreateDate = DateTime.Now
                    };

                    _dbContext.PA_TemplateDesignations.Add(newDesignation);
                }

                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;
                model.Messsage = "Template designation assigned successfully.";
               
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

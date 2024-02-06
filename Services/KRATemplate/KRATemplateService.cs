using System;
using System.Linq;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using DF_EvolutionAPI.Models.Response;

namespace DF_EvolutionAPI.Services.KRATemplate
{
    public class KRATemplateService : IKRATemplateService
    {
        private readonly DFEvolutionDBContext _dbContext;
        public KRATemplateService(DFEvolutionDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //For inserting the templates of KRA's
        public async Task<ResponseModel> CreateKraTemplate(PATemplates paTemplates)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                paTemplates.IsActive = true;
                paTemplates.CreateBy = 1;
                paTemplates.UpdateBy = 1;
                paTemplates.CreateDate = DateTime.Now;
                paTemplates.UpdateDate = DateTime.Now;

                _dbContext.Add(paTemplates);
                model.Messsage = "Template Saved Successfully.";

                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error :" + ex.Message;
            }
            return model;
        }

        //For Updating the template for kra's
        public async Task<ResponseModel> UpdateKraTemplate(PATemplates paTemplates)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                PATemplates updatetemplate = _dbContext.PATemplates.Find(paTemplates.TemplateId);
                if (updatetemplate != null)
                {
                    updatetemplate.Name = paTemplates.Name;
                    updatetemplate.Description = paTemplates.Description;
                    updatetemplate.IsActive = true;
                    paTemplates.CreateBy = 1;
                    paTemplates.UpdateBy = 1;
                    paTemplates.CreateDate = DateTime.Now;
                    paTemplates.UpdateDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                    model.Messsage = "Template updated successfully.";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Template does not exist.";
                }
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error =" + ex.Message;

            }
            return model;
        }

        // For Displaying the template through Id.
        public async Task<PATemplates> GetKraTemplatesById(int Id)
        {
            return _dbContext.PATemplates.Where(templates => templates.TemplateId == Id && templates.IsActive == true)
                    .FirstOrDefault();

        }

        //For deleting the template.
        public async Task<ResponseModel> DeleteKraTemplateById(int Id)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteTemplate = _dbContext.PATemplates.Find(Id);
                if (deleteTemplate != null)
                {

                    deleteTemplate.CreateBy = 1;
                    deleteTemplate.UpdateBy = 1;
                    deleteTemplate.IsActive = false;
                    deleteTemplate.CreateDate = DateTime.Now;
                    deleteTemplate.UpdateDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                    model.Messsage = "Template deleted successfully.";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Template not found.";
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

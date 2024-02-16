using System;
using System.Linq;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using DF_EvolutionAPI.Models.Response;
//using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using DF_EvolutionAPI.Models;
using System.Net;
//using System.Data.Entity;

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
        public async Task<ResponseModel> CreateKraTemplate(PATemplate paTemplates)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                paTemplates.IsActive = 1;
                paTemplates.CreateBy = 1;
                paTemplates.CreateDate = DateTime.Now;

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
        public async Task<ResponseModel> UpdateKraTemplate(PATemplate paTemplates)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                PATemplate updatetemplate = _dbContext.PATemplates.Find(paTemplates.TemplateId);
                if (updatetemplate != null)
                {
                    updatetemplate.Name = paTemplates.Name;
                    updatetemplate.Description = paTemplates.Description;
                    updatetemplate.IsActive = 1;
                    paTemplates.UpdateBy = 1;
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
        public async Task<PATemplate> GetKraTemplateByIdDetails(int templateId)
        {
            var template = await _dbContext.PATemplates
                    .Include(template => template.AssignedKras.Where(kra => kra.IsActive == 1))
                    //.Include(template => template.AssignedDesignations.Where(assigndesignation => assigndesignation.IsActive == 1))
                    .FirstOrDefaultAsync(t => t.TemplateId == templateId);

            if (template == null)
            {
                return null;
            }

            if (template.AssignedKras != null && template.AssignedKras.Count > 0) 
            {
                foreach (var assignedKra in template.AssignedKras)
                {
                    assignedKra.KraLibrary = await GetKraLibrary(assignedKra.KraId);
                }

                //foreach (var assignedDesignation in template.AssignedDesignations)
                //{
                //    assignedDesignation.Designation = await GetDesignation(assignedDesignation.DesignationId);
                //}
            }

            return template;
        }

        private async Task<KRALibrary> GetKraLibrary(int kraId)
        {
            return await _dbContext.KRALibrary.FindAsync(kraId);
        }

        private async Task<Designation> GetDesignation(int designationId)
        {
            return await _dbContext.Designations.FindAsync(designationId);
        }

        public async Task<PATemplate> GetKraTemplateById(int templateId)
        {
            return await _dbContext.PATemplates.Where(template => template.TemplateId == templateId)
                 .FirstOrDefaultAsync();
        }

        //For displaying all templates
        public async Task<List<PATemplate>> GetAllTemplates()
        {
            return await _dbContext.PATemplates.Where(c => c.IsActive == 1).ToListAsync();

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

                    deleteTemplate.UpdateBy = 1;
                    deleteTemplate.IsActive = 0;
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

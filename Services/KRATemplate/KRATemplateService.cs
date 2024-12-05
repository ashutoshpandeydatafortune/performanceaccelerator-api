using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ResponseModel> CreateKraTemplate(PATemplate paTemplates)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var existingTemplate = _dbContext.PATemplates.FirstOrDefault(template => template.Name == paTemplates.Name && template.IsActive == (int)Status.IS_ACTIVE);

                if (existingTemplate == null)
                {
                    paTemplates.IsActive = (int)Status.IS_ACTIVE;                   
                    paTemplates.CreateDate = DateTime.Now;

                    _dbContext.Add(paTemplates);
                    model.Messsage = "Template Saved Successfully.";

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                }
                else
                {
                    model.Messsage = "Template already exist.";
                    model.IsSuccess = false;

                }
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
                var existingTemplate = _dbContext.PATemplates.FirstOrDefault(template => template.Name == paTemplates.Name && template.FunctionId == paTemplates.FunctionId 
                && template.TemplateId != paTemplates.TemplateId && template.IsActive == (int)Status.IS_ACTIVE);
                if (existingTemplate != null)
                {
                    model.IsSuccess = false;
                    model.Messsage = "KRA Library with the same name already exists.";
                    return model;
                }
                else
                {
                    PATemplate updatetemplate = _dbContext.PATemplates.Find(paTemplates.TemplateId);
                    if (updatetemplate != null)
                    {
                        updatetemplate.Name = paTemplates.Name;
                        updatetemplate.Description = paTemplates.Description;
                        updatetemplate.IsActive = (int)Status.IS_ACTIVE;
                        updatetemplate.UpdateBy = paTemplates.UpdateBy;
                        updatetemplate.UpdateDate = DateTime.Now;
                        updatetemplate.FunctionId = paTemplates.FunctionId;

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
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error =" + ex.Message;

            }
            return model;
        }

        // For Displaying the designation kras details for particular template.
        public async Task<PATemplate> GetKraTemplateByIdDetails(int templateId)
        {
            var template = await _dbContext.PATemplates
                    .Include(template => template.AssignedKras.Where(kra => kra.KraLibrary.IsActive == (int)Status.IS_ACTIVE))
                    .Include(template => template.AssignedDesignations.Where(assignedDesignation => assignedDesignation.IsActive == (int)Status.IS_ACTIVE))
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
            }

            if (template.AssignedDesignations != null && template.AssignedDesignations.Count > 0)
            {
                foreach (var assignedDesignation in template.AssignedDesignations)
                {
                    assignedDesignation.Designation = await GetDesignation(assignedDesignation.DesignationId);

                }
            }

            return template;
        }

        private async Task<KRALibrary> GetKraLibrary(int id)
        {
            return await _dbContext.KRALibrary.Where(kra => kra.Id == id).FirstOrDefaultAsync();
        }

        private async Task<Designation> GetDesignation(int designationId)
        {
            try
            {
                return await (
                    from pr in _dbContext.Designations.Where(x => x.DesignationId == designationId)
                    select new Designation
                    {
                        DesignationId = pr.DesignationId,
                        DesignationName = pr.DesignationName,
                        ReferenceId = pr.ReferenceId,
                        IsActive = pr.IsActive,
                        CreateBy = pr.CreateBy,
                        UpdateBy = pr.UpdateBy,
                        CreateDate = pr.CreateDate,
                        UpdateDate = pr.UpdateDate,
                    }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Retrieves the template details.
        public async Task<PATemplate> GetKraTemplateById(int templateId)
        {
            return await _dbContext.PATemplates.Where(template => template.TemplateId == templateId)
                 .FirstOrDefaultAsync();
        }

        //Retrieves all templates
        public async Task<List<PATemplate>> GetAllTemplates()
        {
            //return await _dbContext.PATemplates.Where(c => c.IsActive == 1).ToListAsync();
            var query = from template in _dbContext.PATemplates
                        join function in _dbContext.TechFunctions
                        on template.FunctionId equals function.FunctionId
                        where template.IsActive == (int)Status.IS_ACTIVE
                        select new PATemplate
                        {
                            TemplateId = template.TemplateId,
                            Name = template.Name,
                            Description = template.Description,
                            IsActive = template.IsActive,
                            CreateBy = template.CreateBy,
                            UpdateBy = template.UpdateBy,
                            CreateDate = template.CreateDate,
                            UpdateDate = template.UpdateDate,
                            FunctionId = template.FunctionId,
                            FunctionName = function.FunctionName
                        };

            return await query.ToListAsync();

        }

        //Delete the template.
        public async Task<ResponseModel> DeleteKraTemplateById(int id)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteTemplate = _dbContext.PATemplates.Find(id);
                if (deleteTemplate != null)
                {

                    deleteTemplate.UpdateBy = 1;
                    deleteTemplate.IsActive = (int)Status.IN_ACTIVE;
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

        //Assign designation for particular templates.
        public async Task<ResponseModel> AssignDesingations(PATtemplateDesignationList paTemplateDesignation)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var existingRecords = _dbContext.PA_TemplateDesignations.Where(template => template.TemplateId == paTemplateDesignation.TemplateId).ToList();

                // Set IsActive to 0 for all existing record to mark them as inactive
                foreach (var existingRecord in existingRecords)
                {
                    existingRecord.IsActive = (int)Status.IN_ACTIVE;
                    existingRecord.UpdateBy = paTemplateDesignation.CreateBy;
                    existingRecord.UpdateDate = DateTime.Now;
                    _dbContext.PA_TemplateDesignations.Update(existingRecord);
                }

                //Inserting the new reccord.
                if (paTemplateDesignation.DesignationIds != null && paTemplateDesignation.DesignationIds.Any())

                {
                    foreach (var designationId in paTemplateDesignation.DesignationIds)
                    {
                        if (designationId != 0)
                        {
                            var newDesignation = new PATemplateDesignation
                            {
                                TemplateId = paTemplateDesignation.TemplateId,
                                DesignationId = designationId,
                                IsActive = (int)Status.IS_ACTIVE,
                                CreateBy = paTemplateDesignation.CreateBy,
                                CreateDate = DateTime.Now
                            };

                            _dbContext.PA_TemplateDesignations.Add(newDesignation);
                        }
                    }
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

        //Assign Kras for particular template.
        public async Task<ResponseModel> AssignKRAs(PATtemplateKrasList paTemplateKras)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var existingRecords = _dbContext.PA_TemplateKras.Where(template => template.TemplateId == paTemplateKras.TemplateId).ToList();

                // Set IsActive to 0 for each existing record to mark them as inactive
                foreach (var existingRecord in existingRecords)
                {
                    existingRecord.IsActive = (int)Status.IN_ACTIVE;
                    existingRecord.UpdateBy = paTemplateKras.CreateBy;
                    existingRecord.UpdateDate = DateTime.Now;
                    _dbContext.PA_TemplateKras.Update(existingRecord);
                }
                // Assuming paTemplateKras.KraId is a collection of Kra IDs
                if (paTemplateKras.KraIds != null && paTemplateKras.KraIds.Any())
                {
                    foreach (var kraid in paTemplateKras.KraIds)
                    {
                        if (kraid != 0)
                        {
                            var newTemplateKras = new PATemplateKra
                            {
                                TemplateId = paTemplateKras.TemplateId,
                                KraId = kraid,
                                CreateBy = paTemplateKras.CreateBy,
                                IsActive = (int)Status.IS_ACTIVE,
                                CreateDate = DateTime.Now
                            };
                            _dbContext.PA_TemplateKras.Add(newTemplateKras);
                        }
                    }
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

        //Retrieves the list of assigned kras for particular designation.
        public async Task<List<object>> GetAssignedKRAsByDesignationId(int designationId)
        {
            var assignedKRAs = await _dbContext.PA_TemplateDesignations
                .Include(d => d.Designation)
                .Where(d => d.DesignationId == designationId && d.IsActive == (int)Status.IS_ACTIVE)
                .Join(_dbContext.PATemplates,
                d => d.TemplateId,
                t => t.TemplateId,
                (d, t) => new { Designation = d, Template = t })
                .SelectMany(dt => _dbContext.PA_TemplateKras
                .Include(k => k.KraLibrary)
                .Where(k => k.TemplateId == dt.Designation.TemplateId && k.KraLibrary.IsActive == (int)Status.IS_ACTIVE && dt.Template.IsActive == (int)Status.IS_ACTIVE)
                .Select(k => new
                {
                    kraIds = k.KraLibrary.Id,
                    kraNames = k.KraLibrary.Name
                }))
                .GroupBy(k => new { k.kraIds, k.kraNames })
                .Select(group => new
                {
                    kraId = group.Key.kraIds,
                    kraName = group.Key.kraNames
                })
            .ToListAsync();


            if (assignedKRAs == null || assignedKRAs.Count == 0)
            {
                return new List<object>();
            }
            return assignedKRAs.Cast<object>().ToList();

        }

        //Retrieves a list of KRAs assigned to users of a specific designation
        public async Task<List<UserKraResult>> GetAssignedUserKrasByDesignationId(int designationId)
        {
            var result = await (from u in _dbContext.UserKRA
                                join r in _dbContext.Resources on u.UserId equals r.ResourceId
                                join d in _dbContext.PA_TemplateDesignations on r.DesignationId equals d.DesignationId
                                where d.DesignationId == designationId && u.FinalComment == null
                                group u by new { u.UserId, u.KRAId } into g
                                select new UserKraResult
                                {
                                    UserId = g.Key.UserId,
                                    KraId = g.Key.KRAId
                                }).ToListAsync();
            return result;
        }
    }
}

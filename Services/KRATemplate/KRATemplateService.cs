using System;
using System.Linq;
using System.Data;
using DF_PA_API.Models;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DF_EvolutionAPI.Models.Response;

namespace DF_EvolutionAPI.Services.KRATemplate
{
    public class KRATemplateService : IKRATemplateService
    {
        private readonly DFEvolutionDBContext _dbContext;
        private readonly ILogger<KRATemplateService> _logger;
        public KRATemplateService(DFEvolutionDBContext dbContext, ILogger<KRATemplateService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
                    _logger.LogInformation("{Class}.{Method} - No existing template found. Creating new template.", nameof(ResponseModel), nameof(CreateKraTemplate));
                   
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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }
            return model;
        }

        //For Updating the template for kra's
        public async Task<ResponseModel> UpdateKraTemplate(PATemplate paTemplates)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ResponseModel), nameof(UpdateKraTemplate));
            ResponseModel model = new ResponseModel();
            try
            {
                var existingTemplate = _dbContext.PATemplates.FirstOrDefault(template => template.Name == paTemplates.Name && template.FunctionId == paTemplates.FunctionId
                && template.TemplateId != paTemplates.TemplateId && template.IsActive == (int)Status.IS_ACTIVE);
                if (existingTemplate != null)
                {
                    // Log if template already exists
                    _logger.LogWarning("Template with the same name already exists: {TemplateName}", paTemplates.Name);

                    model.IsSuccess = false;
                    model.Messsage = "KRA Library with the same name already exists.";
                    return model;
                }
                else
                {
                    // Log that we're attempting to update the template
                    _logger.LogInformation("Starting the update process for template with ID: {TemplateId}", paTemplates.TemplateId);

                    PATemplate updatetemplate = _dbContext.PATemplates.Find(paTemplates.TemplateId);
                    if (updatetemplate != null)
                    {
                        // Log before updating template fields
                        _logger.LogInformation("Updating template with ID: {TemplateId}", paTemplates.TemplateId);

                        updatetemplate.Name = paTemplates.Name;
                        updatetemplate.Description = paTemplates.Description;
                        updatetemplate.IsActive = (int)Status.IS_ACTIVE;
                        updatetemplate.UpdateBy = paTemplates.UpdateBy;
                        updatetemplate.UpdateDate = DateTime.Now;
                        updatetemplate.FunctionId = paTemplates.FunctionId;

                        await _dbContext.SaveChangesAsync();

                        // Log successful update
                        _logger.LogInformation("Template with ID {TemplateId} updated successfully.", paTemplates.TemplateId);

                        model.IsSuccess = true;
                        model.Messsage = "Template updated successfully.";
                    }
                    else
                    {
                        // Log if template not found
                        _logger.LogWarning("Template with ID {TemplateId} does not exist.", paTemplates.TemplateId);

                        model.IsSuccess = false;
                        model.Messsage = "Template does not exist.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

            }
            return model;
        }

        // For Displaying the designation kras details for particular template.
        public async Task<PATemplate> GetKraTemplateByIdDetails(int templateId)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(PATemplate), nameof(GetKraTemplateByIdDetails));
          
            try
            {
                _logger.LogInformation("Entering method for template ID: {TemplateId}", templateId);

                var template = await _dbContext.PATemplates
                    .Include(template => template.AssignedKras.Where(kra =>
                        kra.KraLibrary.IsActive == (int)Status.IS_ACTIVE &&
                        kra.IsActive == (int)Status.IS_ACTIVE))
                    .Include(template => template.AssignedDesignations.Where(assignedDesignation =>
                        assignedDesignation.IsActive == (int)Status.IS_ACTIVE))
                    .FirstOrDefaultAsync(t => t.TemplateId == templateId);

                if (template == null)
                {
                    //_logger.LogWarning("Template not found. TemplateId: {TemplateId}", templateId);
                    return null;
                }

                if (template.AssignedKras?.Count > 0)
                {
                    foreach (var assignedKra in template.AssignedKras)
                    {
                        assignedKra.KraLibrary = await GetKraLibrary(assignedKra.KraId);
                        //_logger.LogDebug("Loaded KraLibrary for KraId: {KraId}", assignedKra.KraId);
                    }
                }

                if (template.AssignedDesignations?.Count > 0)
                {
                    foreach (var assignedDesignation in template.AssignedDesignations)
                    {
                        assignedDesignation.DesignatedRole = await GetDesignatedRoles(assignedDesignation.DesignatedRoleId);
                        _logger.LogDebug("Loaded DesignatedRole for RoleId: {RoleId}", assignedDesignation.DesignatedRoleId);
                    }
                }

                _logger.LogInformation("Successfully retrieved template with ID: {TemplateId}", templateId);
                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }


        private async Task<KRALibrary> GetKraLibrary(int id)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(KRALibrary), nameof(GetKraLibrary));
           
            try
            {
                _logger.LogInformation("Fetching KRA Library with ID: {KraId}", id);

                var result = await _dbContext.KRALibrary.Where(kra => kra.Id == id & kra.IsActive == (int)Status.IS_ACTIVE).FirstOrDefaultAsync();

                if (result == null)
                {
                    _logger.LogWarning("No KRA Library found for ID: {KraId}", id);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved KRA Library for ID: {KraId}", id);
                }

                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
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
            catch (Exception ex)
            {
                throw;
            }
        }


        private async Task<DesignatedRole> GetDesignatedRoles(int designatedRoleId)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(DesignatedRole), nameof(GetDesignatedRoles));
            _logger.LogInformation("Fetching DesignatedRole with ID: {RoleId}", designatedRoleId);
            try
            {
                var result = await _dbContext.DesignatedRoles.Where(x => x.DesignatedRoleId == designatedRoleId).FirstOrDefaultAsync();

                if (result == null)
                {
                    _logger.LogWarning("No DesignatedRole found for ID: {RoleId}", designatedRoleId);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved DesignatedRole for ID: {RoleId}", designatedRoleId);
                }

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }


        //Retrieves the template details.
        public async Task<PATemplate> GetKraTemplateById(int templateId)

        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(PATemplate), nameof(GetKraTemplateById));

            return await _dbContext.PATemplates.Where(template => template.TemplateId == templateId)
                 .FirstOrDefaultAsync();
        }

        //Retrieves all templates
        public async Task<List<PATemplate>> GetAllTemplates()
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(PATemplate), nameof(GetAllTemplates));
            try
            {               
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

               
                var result = await query.OrderBy(templateName => templateName.Name).ToListAsync();
                _logger.LogInformation("Retrieved {Count} active templates.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;

            }

        }

        //Delete the template.
        public async Task<ResponseModel> DeleteKraTemplateById(int id)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ResponseModel), nameof(DeleteKraTemplateById));

            ResponseModel model = new ResponseModel();
            try
            {
               
                var deleteTemplate = _dbContext.PATemplates.Find(id);
                if (deleteTemplate != null)
                {
                    _logger.LogInformation("Template found with ID: {Id}", id);

                    deleteTemplate.UpdateBy = 1;
                    deleteTemplate.IsActive = (int)Status.IN_ACTIVE;
                    deleteTemplate.UpdateDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                    model.Messsage = "Template deleted successfully.";

                    _logger.LogInformation("Template deleted successfully with ID: {Id}", id);
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Template not found.";
                    _logger.LogWarning("Template with ID {Id} not found", id);
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

            }
            return model;
        }

        //Assign designation for particular templates.
        public async Task<ResponseModel> AssignDesingations(PATtemplateDesignationList paTemplateDesignation)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ResponseModel), nameof(AssignDesingations));
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
                if (paTemplateDesignation.DesignatedRoleIds != null && paTemplateDesignation.DesignatedRoleIds.Any())
                {
                    foreach (var designatedRoleId in paTemplateDesignation.DesignatedRoleIds)
                    {
                        if (designatedRoleId != 0)
                        {
                            var newDesignation = new PATemplateDesignation
                            {
                                TemplateId = paTemplateDesignation.TemplateId,
                                DesignationId = null,// It is set 0 because we using designatedRoleId
                                DesignatedRoleId = designatedRoleId,
                                IsActive = (int)Status.IS_ACTIVE,
                                CreateBy = paTemplateDesignation.CreateBy,
                                CreateDate = DateTime.Now
                            };

                            _dbContext.PA_TemplateDesignations.Add(newDesignation);

                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No DesignatedRoleIds provided for TemplateId: {TemplateId}", paTemplateDesignation.TemplateId);
                }

                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;
                model.Messsage = "Template designation assigned successfully.";

            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.InnerException = ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString();
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }
            return model;
        }

        //Assign Kras for particular template.
        public async Task<ResponseModel> AssignKRAs(PATtemplateKrasList paTemplateKras)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(ResponseModel), nameof(AssignKRAs));

            ResponseModel model = new ResponseModel();
            try
            {
               
                var existingRecords = _dbContext.PA_TemplateKras.Where(template => template.TemplateId == paTemplateKras.TemplateId).ToList();

                _logger.LogInformation("Found {Count} existing records for TemplateId: {TemplateId}", existingRecords.Count, paTemplateKras.TemplateId);
                _logger.LogDebug("Existing records: {@ExistingRecords}", existingRecords);
                // Set IsActive to 0 for each existing record to mark them as inactive
                foreach (var existingRecord in existingRecords)
                {
                    existingRecord.IsActive = (int)Status.IN_ACTIVE;
                    existingRecord.UpdateBy = paTemplateKras.CreateBy;
                    existingRecord.UpdateDate = DateTime.Now;
                    _dbContext.PA_TemplateKras.Update(existingRecord);

                    //_logger.LogDebug("Updated existing record: {@UpdatedRecord}", existingRecord);
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

                            _logger.LogInformation("Added new KRA with KraId: {KraId} to TemplateId: {TemplateId}", kraid, paTemplateKras.TemplateId);

                            _logger.LogDebug("New KRA entity: {@NewKra}", newTemplateKras);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No KraIds provided for TemplateId: {TemplateId}", paTemplateKras.TemplateId);
                }
                // Save changes to the database
                await _dbContext.SaveChangesAsync();
                model.IsSuccess = true;
                //model.Messsage = "Template Kras saved successfully.";                
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }
            return model;
        }

        //Retrieves the list of assigned kras for particular designation.
        public async Task<List<object>> GetAssignedKRAsByDesignationId(int designationId)
        {
            try
            {
                var assignedKRAs = await _dbContext.PA_TemplateDesignations
                    .Include(d => d.DesignatedRole)
                    .Where(d => d.DesignatedRoleId == designationId && d.IsActive == (int)Status.IS_ACTIVE)
                    .Join(_dbContext.PATemplates,
                    d => d.TemplateId,
                    t => t.TemplateId,
                    (d, t) => new { DesignationRole = d, Template = t })
                    .SelectMany(dt => _dbContext.PA_TemplateKras
                    .Include(k => k.KraLibrary)
                    .Where(k => k.TemplateId == dt.DesignationRole.TemplateId && k.KraLibrary.IsActive == (int)Status.IS_ACTIVE && k.IsActive == (int)Status.IS_ACTIVE && dt.Template.IsActive == (int)Status.IS_ACTIVE)
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
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return new List<object>();
            }

        }

        //Retrieves a list of KRAs assigned to users of a specific designatedRoleId
        public async Task<List<UserKraResult>> GetAssignedUserKrasByDesignationId(int designationId)
        {
            try
            {
                var result = await (from u in _dbContext.UserKRA
                                    join r in _dbContext.Resources on u.UserId equals r.ResourceId
                                    join d in _dbContext.PA_TemplateDesignations on r.DesignatedRoleId equals d.DesignatedRoleId
                                    where d.DesignatedRoleId == designationId && u.IsApproved == 0 //&& u.FinalComment == null
                                    group u by new { u.UserId, u.KRAId } into g
                                    select new UserKraResult
                                    {
                                        UserId = g.Key.UserId,
                                        KraId = g.Key.KRAId
                                    }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
            }
        }
    }
}

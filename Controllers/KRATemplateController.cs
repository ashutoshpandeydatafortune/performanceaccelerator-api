﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRATemplate;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Utils;


namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KRATemplateController : Controller
    {
        private IKRATemplateService _kraTemplateService;
        private readonly ILogger<KRATemplateController> _logger;

        public KRATemplateController(IKRATemplateService kraTemplateService, ILogger<KRATemplateController> logger)
        {
            _kraTemplateService = kraTemplateService;
            _logger = logger;
        }

        /// <summary>
        /// Insert the template in the table PA_Templates.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult>CreateKraTemplate(PATemplate paTemplates)
        {
            try
            {
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _kraTemplateService.CreateKraTemplate(paTemplates);
                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }            
        }      

        /// <summary>
        /// Update the Template fo KRAs.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateKraTemplate(PATemplate paTemplates)
        {
            try
            {
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _kraTemplateService.UpdateKraTemplate(paTemplates);
                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }            
        }

        /// <summary>
        /// Get templates through Id.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetKraTemplateByIdDetails/{templateId}")]
        public async Task<IActionResult> GetKraTemplateByIdDetails(int templateId)
        {
            try
            {
               
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);

                var result = await _kraTemplateService.GetKraTemplateByIdDetails(templateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }           
        }

        /// <summary>
        /// Get templates through Id.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetKraTemplateById/{templateId}")]
        public async Task<IActionResult> GetKraTemplateById(int templateId)
        {
            try
            {
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _kraTemplateService.GetKraTemplateById(templateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Getting all the templates.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetAllTemplates()
        {
            try
                
            {
               
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _kraTemplateService.GetAllTemplates();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }            
        }

        /// <summary>
        /// Delete template through Id.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteKraTemplateById/{id}")]
        public async Task<IActionResult> DeleteKraTemplateById(int id)
        {
            try
            {
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _kraTemplateService.DeleteKraTemplateById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }            
        }

        /// <summary>
        /// Assign the template to designation in the table PA_TemplateDesignation and inactive re designation for particular template.
        /// </summary>
        /// <param name="PATemplateDesignation"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> AssignDesingations(PATtemplateDesignationList paTemplateDesignation)
        {
            try
            {
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _kraTemplateService.AssignDesingations(paTemplateDesignation);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Assign the template to Kras in the table PA_TemplateKras and inactive all the rest kras of particular template.
        /// </summary>
        /// <param name="PATemplateKras"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> AssignKRAs(PATtemplateKrasList paTemplateKras)
        {
            try
            {
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _kraTemplateService.AssignKRAs(paTemplateKras);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Displaying Kras list of for particular designation..
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAssignedKRAsByDesignationId/{designationId}")]
        public async Task<IActionResult> GetAssignedKRAsByDesignationId(int designationId)
        {
            try
            {
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _kraTemplateService.GetAssignedKRAsByDesignationId(designationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Displaying Kras list of for particular designation..
        /// </summary>
        /// <param designationId=""></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAssignedUserKrasByDesignationId/{designationId}")]
        public async Task<IActionResult> GetAssignedUserKrasByDesignationId(int designationId)
        {
            try
            {
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _kraTemplateService.GetAssignedUserKrasByDesignationId(designationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRATemplate;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KRATemplateController : Controller
    {
        private IKRATemplateService _kraTemplateService;

        public KRATemplateController(IKRATemplateService kraTemplateService )
        {
            _kraTemplateService = kraTemplateService;
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
                var response = await _kraTemplateService.CreateKraTemplate(paTemplates);
                return Ok(response);
            }
            catch(Exception ex)
            {
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
                var response = await _kraTemplateService.UpdateKraTemplate(paTemplates);
                return Ok(response);
            }
            catch(Exception ex)
            {
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
                var result = await _kraTemplateService.GetKraTemplateByIdDetails(templateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
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
                var result = await _kraTemplateService.GetKraTemplateById(templateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
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
                var result = await _kraTemplateService.GetAllTemplates();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        /// <summary>
        /// Delete template through Id.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteKraTemplateById/{Id}")]
        public async Task<IActionResult> DeleteKraTemplateById(int Id)
        {
            try
            {
                var result = await _kraTemplateService.DeleteKraTemplateById(Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}

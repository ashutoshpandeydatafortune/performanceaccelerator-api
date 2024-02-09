using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRATemplateDesignation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KraTemplateDesignationController : Controller
    {
        private IKraTemplateDesignation _kraTemplateDesignation;
        
       public KraTemplateDesignationController(IKraTemplateDesignation kraTemplateDesignation)
        {
           _kraTemplateDesignation = kraTemplateDesignation;
        }

        /// <summary>
        /// Assign the template to designation in the table PA_TemplateDesignation.
        /// </summary>
        /// <param name="PATemplateDesignation"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateTemplateDesingation(PATemplateDesignation paTemplateDesignation)
        {
            try
            {
                var response = await _kraTemplateDesignation.AssignTemplateDesingation(paTemplateDesignation);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        /// <summary>
        /// Unassign the template to designation by inactive the entry.
        /// </summary>
        /// <param name="templateDesignationId"></param>
        /// <returns></returns>

        [HttpDelete]
        [Route("UnassignTemplateDesignation/{templateDesignationId}")]
        public async Task<IActionResult> UnassignTemplateDesignation(int templateDesignationId)
        {
            try
            {
                var response = await _kraTemplateDesignation.UnassignTemplateDesignation(templateDesignationId);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}

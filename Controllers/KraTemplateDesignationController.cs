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
                var response = await _kraTemplateDesignation.AssignDesingations(paTemplateDesignation);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}

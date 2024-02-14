using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRATemplateDesignation;
using DF_EvolutionAPI.Services.KRATemplateKras;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KraTemplateKrasController : Controller
    {
        private IKraTemplateKras _kraTemplateKras;

        public KraTemplateKrasController(IKraTemplateKras kraTemplateKras)
        {
            _kraTemplateKras = kraTemplateKras;
        }

        /// <summary>
        /// Assign the template to Kras in the table PA_TemplateKras and inactive all the rest kras of particular template.
        /// </summary>
        /// <param name="PATemplateKras"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> AssignTemplateKras(PATtemplateKrasList paTemplateKras)
        {
            try
            {
                var response = await _kraTemplateKras.AssignTemplateKras(paTemplateKras);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }

    }
}

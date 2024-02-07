using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRATemplateDesignation;
using Microsoft.AspNetCore.Mvc;
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
        /// Insert the template in the table PA_TemplateDesignation.
        /// </summary>
        /// <param name="PATemplateDesignation"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateTemplateDesingation(PATemplateDesignation paTemplateDesignation)
        {
            var response = await _kraTemplateDesignation.CreateTemplateDesingation(paTemplateDesignation);
            return Ok(response);
        }
    }
}

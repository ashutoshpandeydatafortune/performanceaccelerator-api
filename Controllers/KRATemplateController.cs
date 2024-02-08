using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.KRATemplate;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult>CreateKraTemplate(PATemplates paTemplates)
        {
            var response = await _kraTemplateService.CreateKraTemplate(paTemplates);
            return Ok(response);
        }      

        /// <summary>
        /// Update the Template fo KRAs.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateKraTemplate(PATemplates paTemplates)
        {
            var response = await _kraTemplateService.UpdateKraTemplate(paTemplates);
            return Ok(response);
        }

        /// <summary>
        /// Get templates through Id.
        /// </summary>
        /// <param name="PATemplates"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetKraTemplateById/{Id}")]
        public async Task<IActionResult> GetKraTemplateById(int Id)
        {
            var result = await _kraTemplateService.GetKraTemplatesById(Id);
            return Ok(result);
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
            var result = await _kraTemplateService.GetAllTemplates();
            return Ok(result);
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
            var result = await _kraTemplateService.DeleteKraTemplateById(Id);
                return Ok(result);
        }

    }
}

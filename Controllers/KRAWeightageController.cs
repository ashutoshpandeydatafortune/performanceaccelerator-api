using System;
using Microsoft.AspNetCore.Mvc;

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KRAWeightageController : ControllerBase
    {
        private IKRAWeightageService _kraWeightageService;

        public KRAWeightageController(IKRAWeightageService kraWeightageService)
        {
            _kraWeightageService = kraWeightageService;
        }

        /// <summary>
        /// get all kraweightages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllKRAWeightages()
        {
            try
            {
                var weightages = await _kraWeightageService.GetAllKRAWeightageList();
                return Ok(weightages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get weightage details by id
        /// </summary>
        /// <param name="weightageid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/weightageid")]
        public async Task<IActionResult> GetKRAWeightageById(int weightageId)
        {
            try
            {
                var weightage = await _kraWeightageService.GetKRAWeightageDetailsById(weightageId);

                if (weightage == null) return NotFound();
                
                return Ok(weightage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// save weightage
        /// </summary>
        /// <param name="weightageModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateorUpdateKRAWeightage(KRAWeightage weightageModel)
        {
            try
            {
                var model = await _kraWeightageService.CreateorUpdateKRAWeightage(weightageModel);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete weightage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteKRAWeightage(int id)
        {
            try
            {
                var model = await _kraWeightageService.DeleteKRAWeightage(id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

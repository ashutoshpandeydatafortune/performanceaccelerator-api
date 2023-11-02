using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KRAWeightageController : ControllerBase
    {
        IKRAWeightageService _kraWeightageService;
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
        public IActionResult GetAllKRAWeightages()
        {
            try
            {
                var weightages = _kraWeightageService.GetAllKRAWeightageList();
                if (weightages.Result == null) return NotFound();
                return Ok(weightages.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// get weightage details by id
        /// </summary>
        /// <param name="weightageid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/weightageid")]
        public IActionResult GetKRAWeightageById(int weightageId)
        {
            try
            {
                var weightages = _kraWeightageService.GetKRAWeightageDetailsById(weightageId);
                if (weightages.Result == null) return NotFound();
                return Ok(weightages.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// save weightage
        /// </summary>
        /// <param name="weightageModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateKRAWeightage(KRAWeightage weightageModel)
        {
            try
            {
                var model = _kraWeightageService.CreateorUpdateKRAWeightage(weightageModel);
                return Ok(model.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// delete weightage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteKRAWeightage(int id)
        {
            try
            {
                var model = _kraWeightageService.DeleteKRAWeightage(id);
                return Ok(model.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }
    }
}

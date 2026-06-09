using System;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubSkillController : Controller
    {

       private ISubSkillService _subSkillService;
        private readonly ILogger<SubSkillController> _logger;

        public SubSkillController(ISubSkillService subSkillService, ILogger<SubSkillController> logger)
        {
            _subSkillService = subSkillService;
            _logger = logger;
        }

        /// <summary>
        /// It is used to insert the Subskill.
        /// </summary>
        /// <param name="subSkillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateSubSkill(SubSkill subSkillModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _subSkillService.CreateSubSkill(subSkillModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to update the Subskill.
        /// </summary>
        /// <param name="subSkillModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateBySubSkillId(SubSkill subSkillModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _subSkillService.UpdateBySubSkillId(subSkillModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to get all the Subskill.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetAllSubSkills()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _subSkillService.GetAllSubSkills();
                return Ok(response);
            }

            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to fetch Subskill by id.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSubSkillById/{id}")]
        public async Task<IActionResult> GetSubSkillById(int id)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _subSkillService.GetSubSkillById(id);
                return Ok(response);
            }

            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to delete the subskill.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteSubSkillById/{id}")]
        public async Task<IActionResult> DeleteSubSkillById(int id)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _subSkillService.DeleteSubSkillById(id);
                return Ok(response);
            }

            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }
    }
}

using System;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : Controller
    {
        private ISkillService _skillService;
        private readonly ILogger<SkillController> _logger;
       
        public SkillController(ISkillService skillService, ILogger<SkillController> logger)
        {
            _skillService = skillService;
            _logger = logger;
        }

        /// <summary>
        /// It is used to insert the skill.
        /// </summary>
        /// <param name="skillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateSkill(Skill skillModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _skillService.CreateSkill(skillModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to update the skill.
        /// </summary>
        /// <param name="skillModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateBySkillId(Skill skillModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _skillService.UpdateBySkillId(skillModel);
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
        /// It is used to get all the skill.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetAllSkills()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _skillService.GetAllSkills();
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
        /// It is used to fetch skill by id.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSkillById/{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _skillService.GetSkillById(id);
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
        /// It is used to delete the skill.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteSkillById/{id}")]
        public async Task<IActionResult> DeleteSkillById(int id)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _skillService.DeleteSkillById(id);
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
        /// Search resources by skills
        /// </summary>
        /// <param name="skillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> SearchBySkills(SearchSkill searchSkillModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _skillService.SearchBySkills(searchSkillModel);
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
        /// It is used to fetch skill by category.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSkillByCategoryId/{id}")]
        public async Task<IActionResult> GetSkillByCategoryId(int id)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _skillService.GetSkillByCategoryId(id);
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

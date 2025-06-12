using System;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Services.Submission;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionStatusController : Controller
    {
        private ISubmissionStatusService _submissionStatusService;
        private readonly ILogger<SubmissionStatusController> _logger;

        public SubmissionStatusController(ISubmissionStatusService submissionStatusService, ILogger<SubmissionStatusController> logger)
        {
            _submissionStatusService = submissionStatusService;
            _logger = logger;
        }

        /// <summary>
        /// get all submission statuses list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllSubmissionStatusList()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var submissionstatuses = await _submissionStatusService.GetAllSubmissionStatusList();
                return Ok(submissionstatuses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get submission status by id
        /// </summary>
        /// <param name="submissionStatusId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/submissionStatusId")]
        public async Task<IActionResult> GetSubmissionStatusById(int submissionStatusId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var submissionstatus = await _submissionStatusService.GetSubmissionStatusById(submissionStatusId);
                
                if (submissionstatus == null) return NotFound();
                
                return Ok(submissionstatus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create or update submission status 
        /// </summary>
        /// <param name="submissionStatusModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateorUpdateSubmissionStatus(SubmissionStatus submissionStatusModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var model = await _submissionStatusService.CreateorUpdateSubmissionStatus(submissionStatusModel);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete submission status
        /// </summary>
        /// <param name="submissionStatusId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteSubmissionStatus(int submissionStatusId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var model = await _submissionStatusService.DeleteSubmissionStatus(submissionStatusId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


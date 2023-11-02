using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using DF_EvolutionAPI.Services.Submission;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionStatusController : Controller
    {
        ISubmissionStatusService _submissionStatusService;
        public SubmissionStatusController(ISubmissionStatusService submissionStatusService)
        {
            _submissionStatusService = submissionStatusService;
        }

        /// <summary>
        /// get all submission statuses list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllSubmissionStatusList()
        {
            try
            {
                var submissionstatuses = _submissionStatusService.GetAllSubmissionStatusList();
                if (submissionstatuses.Result == null) return NotFound();
                return Ok(submissionstatuses.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get submission status by id
        /// </summary>
        /// <param name="submissionStatusId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/submissionStatusId")]
        public IActionResult GetSubmissionStatusById(int submissionStatusId)
        {
            try
            {
                var submissionstatus = _submissionStatusService.GetSubmissionStatusById(submissionStatusId);
                if (submissionstatus.Result == null) return NotFound();
                return Ok(submissionstatus.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Create or update submission status 
        /// </summary>
        /// <param name="submissionStatusModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateSubmissionStatus(SubmissionStatus submissionStatusModel)
        {
            try
            {
                var model = _submissionStatusService.CreateorUpdateSubmissionStatus(submissionStatusModel);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// delete submission status
        /// </summary>
        /// <param name="submissionStatusId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteSubmissionStatus(int submissionStatusId)
        {
            try
            {
                var model = _submissionStatusService.DeleteSubmissionStatus(submissionStatusId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}


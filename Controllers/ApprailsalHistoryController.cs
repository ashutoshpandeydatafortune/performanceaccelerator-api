using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using DF_EvolutionAPI.Services.History;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprailsalHistoryController : Controller
    {
        IAppraisalHistoryService _appraisalHistoryService;
        public ApprailsalHistoryController(IAppraisalHistoryService appraisalHistoryService)
        {
            _appraisalHistoryService = appraisalHistoryService;
        }

        /// <summary>
        /// get all appraisal list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllAppraisalHistoryList()
        {
            try
            {
                var appraisals = _appraisalHistoryService.GetAllAppraisalHistoryList();
                if (appraisals.Result == null) return NotFound();
                return Ok(appraisals.Result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get user appraisal history by id
        /// </summary>
        /// <param name="appraisalHistoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/appraisalHistoryId")]
        public IActionResult GetAppraisalHistoryById(int appraisalHistoryId)
        {
            try
            {
                var appraisalHistory = _appraisalHistoryService.GetAppraisalHistoryById(appraisalHistoryId);
                if (appraisalHistory.Result == null) return NotFound();
                return Ok(appraisalHistory.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// create or update appraisal history details 
        /// </summary>
        /// <param name="appraisalHistoryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateAppraisalHistory(AppraisalHistory appraisalHistoryModel)
        {
            try
            {
                var model = _appraisalHistoryService.CreateorUpdateAppraisalHistory(appraisalHistoryModel);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// delete appraisal history
        /// </summary>
        /// <param name="appraisalHistoryId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteAppraisalHistory(int appraisalHistoryId)
        {
            try
            {
                var model = _appraisalHistoryService.DeleteAppraisalHistory(appraisalHistoryId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///get appraisal history by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAppraisalHistoryByUserId(int userId)
        {
            try
            {
                var model = _appraisalHistoryService.GetAppraisalHistoryByUserId(userId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get user details by appraisal history id
        /// </summary>
        /// <param name="appraisalHistoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserDetailsByAppraisalHistoryId(int appraisalHistoryId)
        {
            try
            {
                var model = _appraisalHistoryService.GetUserDetailsByAppraisalHistoryId(appraisalHistoryId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}


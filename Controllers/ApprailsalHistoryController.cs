using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services.History;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprailsalHistoryController : Controller
    {
        private IAppraisalHistoryService _appraisalHistoryService;

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
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get user appraisal history by id
        /// </summary>
        /// <param name="appraisalHistoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/appraisalHistoryId")]
        public async Task<IActionResult> GetAppraisalHistoryById(int appraisalHistoryId)
        {
            try
            {
                var appraisalHistory = await _appraisalHistoryService.GetAppraisalHistoryById(appraisalHistoryId);

                if (appraisalHistory == null) return Content("{}", "application/json");
                
                return Ok(appraisalHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// create or update appraisal history details 
        /// </summary>
        /// <param name="appraisalHistoryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateorUpdateAppraisalHistory(AppraisalHistory appraisalHistoryModel)
        {
            try
            {
                var model = await _appraisalHistoryService.CreateorUpdateAppraisalHistory(appraisalHistoryModel);
                return Ok(model);
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
        public async Task<IActionResult> DeleteAppraisalHistory(int appraisalHistoryId)
        {
            try
            {
                var model = await _appraisalHistoryService.DeleteAppraisalHistory(appraisalHistoryId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///get appraisal history by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAppraisalHistoryByUserId(int userId)
        {
            try
            {
                var model = await _appraisalHistoryService.GetAppraisalHistoryByUserId(userId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get user details by appraisal history id
        /// </summary>
        /// <param name="appraisalHistoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserDetailsByAppraisalHistoryId(int appraisalHistoryId)
        {
            try
            {
                var model = await _appraisalHistoryService.GetUserDetailsByAppraisalHistoryId(appraisalHistoryId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}


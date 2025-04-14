using System;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using DF_EvolutionAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApprovalController : Controller
    {
        private IUserApprovalService _userApprovalService;
        private readonly ILogger<UserApprovalController> _logger;

        public UserApprovalController(IUserApprovalService userApprovalService, ILogger<UserApprovalController> logger)
        {
            _userApprovalService = userApprovalService;
            _logger = logger;
        }

        /// <summary>
        /// get all user approval list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllUserApprovals()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var approvals = await _userApprovalService.GetAllApprovalList();
                return Ok(approvals);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get user approval by id
        /// </summary>
        /// <param name="userApprovalId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/userApprovalId")]
        public async Task<IActionResult> GetUserApprovalById(int userApprovalId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var status = await _userApprovalService.GetApprovalById(userApprovalId);

                if (status == null) return NotFound();

                return Ok(status);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create or Update User Approval
        /// </summary>
        /// <param name="userApprovalModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateorUpdateUserApproval(List<UserApproval> userApprovalModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                ResponseModel model = new ResponseModel();
                foreach (var item in userApprovalModel)
                {
                    model = await _userApprovalService.CreateorUpdateApproval(item);
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete User Approval
        /// </summary>
        /// <param name="userApprovalId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteUserApproval(int userApprovalId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var model = await _userApprovalService.DeleteApproval(userApprovalId);
                return Ok(model);
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
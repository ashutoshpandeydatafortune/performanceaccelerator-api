using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using DF_EvolutionAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApprovalController : Controller
    {
        private IUserApprovalService _userApprovalService;

        public UserApprovalController(IUserApprovalService userApprovalService)
        {
            _userApprovalService = userApprovalService;
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
                var approvals = await _userApprovalService.GetAllApprovalList();
                return Ok(approvals);
            }
            catch (Exception ex)
            {
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
                var status = await _userApprovalService.GetApprovalById(userApprovalId);

                if (status == null) return NotFound();

                return Ok(status);
            }
            catch (Exception ex)
            {
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
                ResponseModel model = new ResponseModel();
                foreach (var item in userApprovalModel)
                {
                    model = await _userApprovalService.CreateorUpdateApproval(item);
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
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
                var model = await _userApprovalService.DeleteApproval(userApprovalId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
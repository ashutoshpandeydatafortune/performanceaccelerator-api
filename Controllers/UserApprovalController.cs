using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using DF_EvolutionAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApprovalController : Controller
    {
        IUserApprovalService _userApprovalService;
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
        public IActionResult GetAllUserApprovals()
        {
            try
            {
                var approvals = _userApprovalService.GetAllApprovalList();
                if (approvals.Result == null) return NotFound();
                return Ok(approvals.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get user approval by id
        /// </summary>
        /// <param name="userApprovalId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/userApprovalId")]
        public IActionResult GetUserApprovalById(int userApprovalId)
        {
            try
            {
                var status = _userApprovalService.GetApprovalById(userApprovalId);
                if (status.Result == null) return NotFound();
                return Ok(status.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Create or Update User Approval
        /// </summary>
        /// <param name="userApprovalModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateUserApproval(List<UserApproval> userApprovalModel)
        {
            try
            {
                ResponseModel model = new ResponseModel();
                foreach (var item in userApprovalModel)
                {

                    var model1 = _userApprovalService.CreateorUpdateApproval(item);
                    model = model1.Result;

                }

              //  var model = _userApprovalService.CreateorUpdateApproval(userApprovalModel);
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// delete User Approval
        /// </summary>
        /// <param name="userApprovalId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteUserApproval(int userApprovalId)
        {
            try
            {
                var model = _userApprovalService.DeleteApproval(userApprovalId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using DF_EvolutionAPI.Services.KRA;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserKRAController : Controller
    {
        private IUserKRAService _userKRAService;

        public UserKRAController(IUserKRAService userKRAService)
        {
            _userKRAService = userKRAService;
        }

        /// <summary>
        /// get all user KRAs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllUserKRAs()
        {
            try
            {
                var userKRAs = await _userKRAService.GetAllUserKRAList();
                return Ok(userKRAs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get user KRA by id
        /// </summary>
        /// <param name="userKRAId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/userKRAId")]
        public async Task<IActionResult> GetuserKRAById(int userKRAId)
        {
            try
            {
                var userKRA = await _userKRAService.GetUserKRAById(userKRAId);

                if (userKRA == null) return NotFound();

                return Ok(userKRA);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetKRAsByUserId/{UserId}")]
        public async Task<IActionResult> GetKRAsByUserId(int UserId)
        {
            try
            {
                var userKRADetails = await _userKRAService.GetKRAsByUserId(UserId);
                return Ok(userKRADetails);               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAssignedKRAsByDesignation/{designation}")]
        public IActionResult GetAssignedKRAsByDesignation(string designation)
        {
            try
            {
                var assignedKRAs = _userKRAService.GetAssignedKRAsByDesignation(designation);
                return Ok(assignedKRAs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// create or update user KRA
        /// </summary>
        /// <param name="userKRAModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateorUpdateUserKRA(List<UserKRA> userKRAModel)
        {
            try
            {
                ResponseModel model = new ResponseModel();
                foreach (var item in userKRAModel)
                {                    
                    model = await _userKRAService.CreateorUpdateUserKRA(item) ;
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete User KRA
        /// </summary>
        /// <param name="userKRAId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteUserKRA(int userKRAId)
        {
            try
            {
                var model = await _userKRAService.DeleteUserKRA(userKRAId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using DF_EvolutionAPI.Services.KRA;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static DF_EvolutionAPI.Models.UserKRADetails;

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
        public async Task<IActionResult> GetUserKRAById(int userKRAId)
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
        public IActionResult GetKRAsByUserId(int UserId)
        {
            try
            {
                var userKRADetails = _userKRAService.GetKRAsByUserId(UserId);
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
        /// displayed all the kras according to designation Id.
        /// </summary>
        /// <param name="UserAssignedKRA"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAssignedKRAsByDesignationId/{designationId}")]
        public IActionResult GetAssignedKRAsByDesignationId(int designationId)
        {
            try
            {
                var assignedKRAs = _userKRAService.GetAssignedKRAsByDesignationId(designationId);
                return Ok(assignedKRAs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// create user KRA
        /// </summary>
        /// <param name="userKRAModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateUserKRA(List<UserKRA> userKRAModel)
        {
            try
            {
                ResponseModel model = await _userKRAService.CreateUserKRA(userKRAModel);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// update user KRA
        /// </summary>
        /// <param name="userKRAModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateUserKra(UpdateUserKRARequest request)
        {
            try
            {
                ResponseModel model = await _userKRAService.UpdateUserKra(request);

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
        /// <summary>
        /// It displays the rating according to Quarters for particular user. This can be used in for displaying graph
        /// </summary>
        /// <param name="UserId" and YearRange="QuarterYearRange"></param>
        /// <returns>Rating and QuateName</returns>
        [HttpGet]
        [Route("[action]/{userId}/{quarterYearRange?}")]
        public IActionResult GetUserKraGraph(int userId, string quarterYearRange)
        {
            try
            {
                var model = _userKRAService.GetUserKraGraph(userId, quarterYearRange); 
                return Ok(model);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Removed the assigned or unassign kras from resources
        /// </summary>
        /// <param name="userKRAId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AssignUnassignKra(int userKraId, byte IsActive)
        {
            try
            {
                var model = await _userKRAService.AssignUnassignKra(userKraId,IsActive);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
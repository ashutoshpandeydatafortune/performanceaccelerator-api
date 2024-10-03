﻿using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : Controller
    {
        private ISettingsService _settings;
        public SettingsController(ISettingsService settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// get all rolemapping data for particular roleId
        /// </summary>
        /// <returns></returns>
       
        [HttpGet]
        [Route("GetPermissionsByRole/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRole(string roleId)
        {
            try
            {
                var result = await _settings.GetPermissionsByRole(roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updating rolemapping for particular roleId
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> UpdatePermissionByRole(RoleMapping roleMapping)
        {
            try
            {
                var result = await _settings.UpdatePermissionByRole(roleMapping);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Inserting rolemapping for role
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> AddRoleMapping(List<RoleMapping> roleMappings)
        {
            try
            {
                var result = await _settings.AddRoleMapping(roleMappings);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all role
        /// </summary>
        /// <returns></returns>
        /// [HttpGet]
        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var result = await _settings.GetAllRoles();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Create or update the role for user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public IActionResult CreateOrUpdateUserRole(string emailId, string roleName)
        {
            try
            {
                var result = _settings.CreateOrUpdateUserRole(emailId, roleName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

}

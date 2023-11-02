using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        IStatusService _statusService;
        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        /// <summary>
        /// get all statuses
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllStatusList()
        {
            try
            {
                var statuses = _statusService.GetAllStatusList();
                if (statuses.Result == null) return NotFound();
                return Ok(statuses.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get status details by id
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/statusId")]
        public IActionResult GetStatusById(int statusId)
        {
            try
            {
                var status = _statusService.GetStatusById(statusId);
                if (status.Result == null) return NotFound();
                return Ok(status.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// save status details
        /// </summary>
        /// <param name="statusModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateStatus(StatusLibrary statusModel)
        {
            try
            {
                var model = _statusService.CreateorUpdateStatus(statusModel);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// delete status
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteStatus(int statusId)
        {
            try
            {
                var model = _statusService.DeleteStatus(statusId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}


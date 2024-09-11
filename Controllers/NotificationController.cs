using System;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DF_EvolutionAPI.Services;


namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// IT is used to insert the record in PA_Notification table.
        /// </summary>
        /// <param name="notificationModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateNotification(Notification notificationModel)
        {
            try
            {
                var response = await _notificationService.CreateNotification(notificationModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// In this all the record for particular resource is diplayed with update checked IsRead to true.
        /// </summary>  
        /// <param name="resourceId"></param>
        /// <returns> It returns the list of read notifications for a particular resource.</returns>
        [HttpGet]
        [Route("GetNotificationsByResourceId/{resourceId}")]
        public async Task<IActionResult> GetNotificationsByResourceId(int resourceId)
        {
            try
            {
                var result = await _notificationService.GetNotificationsByResourceId(resourceId);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get notification through id.
        /// </summary>  
        /// <param name="Id"></param>
        /// <returns> It returns the notification of particular id.</returns>
        [HttpGet]
        [Route("GetNotificationById/{Id}")]
        public async Task<IActionResult> GetNotificationById(int Id)
        {            
            try
            {
                var result = await _notificationService.GetNotificationById(Id);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

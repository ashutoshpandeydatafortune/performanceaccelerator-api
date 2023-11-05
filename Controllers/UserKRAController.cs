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
        IUserKRAService _userKRAService;
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
        public IActionResult GetAllUserKRAs()
        {
            try
            {
                var userKRAs = _userKRAService.GetAllUserKRAList();
                if (userKRAs.Result == null) return NotFound();
                return Ok(userKRAs.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get user KRA by id
        /// </summary>
        /// <param name="userKRAId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/userKRAId")]
        public IActionResult GetuserKRAById(int userKRAId)
        {
            try
            {
                var userKRA = _userKRAService.GetUserKRAById(userKRAId);
                if (userKRA.Result == null) return NotFound();
                return Ok(userKRA.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetKRAsByUserId/{UserId}")]
        public async Task<IActionResult> GetKRAsByUserId(int UserId)
        {
            try
            {
                var USerKRADetails = await _userKRAService.GetKRAsByUserId(UserId);
                if (USerKRADetails == null) return NoContent();
                return Ok(USerKRADetails);

               
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }



        /// <summary>
        /// create or update user KRA
        /// </summary>
        /// <param name="userKRAModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateUserKRA(List<UserKRA> userKRAModel)
        {
            try
            {
                // Boolean model;
                ResponseModel model = new ResponseModel();
                foreach (var item in userKRAModel)
                {
                    
                    var model1 = _userKRAService.CreateorUpdateUserKRA(item) ;
                    model = model1.Result;
                    
                }

                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// delete User KRA
        /// </summary>
        /// <param name="userKRAId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteUserKRA(int userKRAId)
        {
            try
            {
                var model = _userKRAService.DeleteUserKRA(userKRAId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}


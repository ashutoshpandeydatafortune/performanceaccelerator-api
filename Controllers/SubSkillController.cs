using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubSkillController : Controller
    {

        private ISubSkillService _subSkillService;

        public SubSkillController(ISubSkillService subSkillService)
        {
            _subSkillService = subSkillService;
        }

        /// <summary>
        /// It is used to insert the Subskill.
        /// </summary>
        /// <param name="subSkillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateSubSkill(SubSkill subSkillModel)
        {
            try
            {
                var response = await _subSkillService.CreateSubSkill(subSkillModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to update the Subskill.
        /// </summary>
        /// <param name="subSkillModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateBySubSkillId(SubSkill subSkillModel)
        {
            try
            {
                var response = await _subSkillService.UpdateBySubSkillId(subSkillModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to get all the Subskill.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetAllSubSkills()
        {
            try
            {
                var response = await _subSkillService.GetAllSubSkills();
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to fetch Subskill by id.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSubSkillById/{id}")]
        public async Task<IActionResult> GetSubSkillById(int id)
        {
            try
            {
                var response = await _subSkillService.GetSubSkillById(id);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to delete the subskill.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteSubSkillById/{id}")]
        public async Task<IActionResult> DeleteSubSkillById(int id)
        {
            try
            {
                var response = await _subSkillService.DeleteSubSkillById(id);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

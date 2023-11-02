using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services.KRA;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KRALibraryController : Controller
    {
        IKRALibraryService _kraLibraryService;
        public KRALibraryController(IKRALibraryService kraLibraryService)
        {
            _kraLibraryService = kraLibraryService;
        }

        /// <summary>
        /// get all kra Libraries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllKRALibraryList()
        {
            try
            {
                var Librarys = _kraLibraryService.GetAllKRALibraryList();
                if (Librarys.Result == null) return NotFound();
                return Ok(Librarys.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// get KRA library details by id
        /// </summary>
        /// <param name="kraLibraryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetKRALibraryById/{kraLibraryId}")]
        public IActionResult GetKRALibraryById(int kraLibraryId)
        {
            try
            {
                var Librarys = _kraLibraryService.GetKRALibraryById(kraLibraryId);
                if (Librarys.Result == null) return NotFound();
                return Ok(Librarys.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// save KRA Library
        /// </summary>
        /// <param name="kraLibraryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateKRALibrary(KRALibrary kraLibraryModel)
        {
            try
            {
                var model = _kraLibraryService.CreateorUpdateKRALibrary(kraLibraryModel);
                return Ok(model.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// delete Library
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteKRALibrary(int kraLibraryid)
        {
            try
            {
                var model = _kraLibraryService.DeleteKRALibrary(kraLibraryid);
                return Ok(model.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }


        /// <summary>
        /// Get KRA Weightage Details By KRA Library Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetKRAWeightageDetailsByKRALibraryId(int kraLibraryid)
        {
            try
            {
                var weightagedetails = _kraLibraryService.GetKRAWeightageDetailsByKRALibraryId(kraLibraryid);
                return Ok(weightagedetails.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }


        /// <summary>
        /// Get KRA  Details By Weightage Id 
        /// </summary>
        /// <param name="WeightageId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetKRADetailsByWeightageId(int WeightageId)
        {
            try
            {
                var kraLibraries = _kraLibraryService.GetKRADetailsByWeightageId(WeightageId);
                return Ok(kraLibraries.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }
    }
}

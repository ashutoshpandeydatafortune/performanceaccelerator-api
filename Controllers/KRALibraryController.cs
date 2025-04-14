using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services.KRA;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Utils;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KRALibraryController : Controller
    {
        private IKRALibraryService _kraLibraryService;
        private readonly ILogger<KRALibraryController> _logger;       

        public KRALibraryController(IKRALibraryService kraLibraryService, ILogger<KRALibraryController> logger)
        {
            _kraLibraryService = kraLibraryService;
            _logger = logger;
        }

        /// <summary>
        /// get all kra Libraries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllKRALibraryList(int? isNotSpecial)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var libraries = await _kraLibraryService.GetAllKRALibraryList(isNotSpecial);                
                return Ok(libraries);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get KRA library details by id
        /// </summary>
        /// <param name="kraLibraryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetKRALibraryById/{kraLibraryId}")]
        public async Task<IActionResult> GetKRALibraryById(int kraLibraryId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var library = await _kraLibraryService.GetKRALibraryById(kraLibraryId);

                if (library == null) return NotFound();
                
                return Ok(library);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// save KRA Library
        /// </summary>
        /// <param name="kraLibraryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateorUpdateKRALibrary(KRALibrary kraLibraryModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _kraLibraryService.CreateorUpdateKRALibrary(kraLibraryModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete Library
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteKRALibrary/{kraLibraryid}")]
        public async Task<IActionResult> DeleteKRALibrary(int kraLibraryid)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _kraLibraryService.DeleteKRALibrary(kraLibraryid);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get KRA Weightage Details By KRA Library Id
        /// </summary>
        /// <param name="kraLibraryid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetKRAWeightageDetailsByKRALibraryId(int kraLibraryid)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var kraWeightage = await _kraLibraryService.GetKRAWeightageDetailsByKRALibraryId(kraLibraryid);

                if(kraWeightage == null) return NotFound();

                return Ok(kraWeightage);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get KRA  Details By Weightage Id 
        /// </summary>
        /// <param name="weightageId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetKRADetailsByWeightageId(int weightageId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var kraDetails = await _kraLibraryService.GetKRADetailsByWeightageId(weightageId);
                return Ok(kraDetails);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// get all kras functionwise
        /// </summary>
        /// /// <param name="functionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllKRAsByFunction/{functionId}")]
        public async Task<IActionResult> GetAllKRAsByFunction(int functionId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var kras = await _kraLibraryService.GetAllKRAsByFunction(functionId);
                return Ok(kras);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Gets the KRAs assigned to resources based on KRA ID and Function ID.
        /// </summary>
        /// <param name="kraId">The ID of the KRA.</param>
        /// <param name="functionId">The ID of the Function.</param>
        /// <returns>A list of assigned KRAs for the specified function.</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAssignedUserKras(int kraId, int functionId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var kras = await _kraLibraryService.GetAssignedUserKras(kraId, functionId);
                return Ok(kras);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }
    }
}

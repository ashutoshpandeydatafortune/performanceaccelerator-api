using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Utils;
using System.Reflection;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private ILoginService _loginService;
        private IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILoginService loginService, IConfiguration configuration, ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _configuration = configuration;
            _logger = logger;
        }
       
        [AllowAnonymous]
        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin(UserAuthModel uam)
        {
            //_logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
           
            if (string.IsNullOrWhiteSpace(uam.Username))
            {
               
                return BadRequest(new { isError = true, msgError = "This email is not registered with us!" });
            }
            else if (string.IsNullOrWhiteSpace(uam.Key))
            {
                return BadRequest(new { isError = true, msgError = "Un-authorized User!" });
            }
           
            try
            {
                return Ok(await _loginService.ExternalLogin(uam, _configuration));
            }

            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return StatusCode(500, new ErrorResponse(true, ex.Message, null));
            }
        }
    }
}

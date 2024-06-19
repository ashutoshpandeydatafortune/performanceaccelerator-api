using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.Services.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private ILoginService _loginService;
        private IConfiguration _configuration;

        public LoginController(ILoginService loginService, IConfiguration configuration)
        {
            _loginService = loginService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin(UserAuthModel uam)
        {
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
                return StatusCode(500, new ErrorResponse(true, ex.Message, null));
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        [AllowAnonymous]
        [HttpGet("server/status")]
        public IActionResult Status()
        {
            return Ok("Server is running...");
        }
    }
}

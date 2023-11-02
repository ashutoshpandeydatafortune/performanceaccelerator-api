using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// get all clinets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var clients = await _clientService.GetAllClients();
                if (clients == null) return NotFound();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// get client details by id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/clientId")]
        public IActionResult GetClientById(int clientId)
        {
            try
            {
                var client = _clientService.GetClientByClientId(clientId);
                if (client.Result == null) return NotFound();
                return Ok(client.Result);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }
    }
}

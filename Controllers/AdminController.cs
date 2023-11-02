using DF_EvolutionAPI.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
       
        IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}

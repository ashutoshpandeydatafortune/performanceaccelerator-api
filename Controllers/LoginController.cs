using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
       // private readonly
            IResourceService _ResourceDAL;
       // private readonly UserDAL _UserDAL;
        static HttpClient msClient;
        public LoginController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IResourceService resourceDAL)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _ResourceDAL = resourceDAL;
         //   _UserDAL = userDAL;

            msClient = new HttpClient();
            msClient.DefaultRequestHeaders.Accept.Clear();
            msClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
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
                if (uam.AccountType == "MICROSOFT")
                {
                    var FindUser = await _userManager.FindByEmailAsync(uam.Username);

                    if (FindUser == null)
                    {
                        var registerUser = new IdentityUser
                        {
                            Email = uam.Username,
                            UserName = uam.Username,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            EmailConfirmed = true
                        };
                        var result = await _userManager.CreateAsync(registerUser);
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(registerUser, "Other");
                        }
                        // return Ok(new { Username = user.UserName });
                    }
                    // find user by email id 
                    var user = await _userManager.FindByEmailAsync(uam.Username);
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, uam.Username));
                    var claim = new[] {
                                new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
                            };

                    // get user roles
                    var userRoles = await _userManager.GetRolesAsync(user);
                    IdentityRole userRoleId = new IdentityRole();
                    if (userRoles.Count > 0)
                    {
                        userRoleId = await _roleManager.FindByNameAsync(userRoles[0]);
                    }

                    var signinKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                    int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                    var token = new JwtSecurityToken(
                      issuer: _configuration["Jwt:Site"],
                      audience: _configuration["Jwt:Site"],
                      expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                      signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256Signature),
                      claims: claims
                    );

                    int ReferenceId = 0;
                    int ResourceId = 0;
                    int ReportingTo = 0;
                    string ResourceName = "";
                    //string email = ;
                    //string IsReportingto = "";
                    // string IsSafeGuardPerson = "";

                    Resource ObjResource = await _ResourceDAL.GetResourceByEmailId(user.UserName);
                   // return Ok();
                    if (ObjResource == null || ObjResource.ResourceId == 0)
                    {
                        //  ResourceName = user.UserName;
                        return StatusCode(500, "Resource Not Available !!!");
                    }

                    else
                        ResourceId = ObjResource.ResourceId;
                    ResourceName = ObjResource.ResourceName;
                    ReportingTo = (int)ObjResource.ReportingTo;
                    ReferenceId = ObjResource.ResourceId;
                    // IsSafeGuardPerson = objResource.IsSafeGuardPerson;
                    //if (userRoles.Count > 0)
                    //{
                    //    var Role = userRoles.ToArray();
                    //    if (Role[0] == "Lead" || Role[0] == "Project Manager" || Role[0] == "Portfolio Manager")
                    //    {
                    //        ReferenceId = ObjResource.ResourceId;
                    //    }
                    //}
                    return Ok(
                      new
                      {

                          id = user.Id,
                          username = user.UserName,
                          ResourceName = ResourceName,
                          ResourceId = ResourceId,
                          ReferenceId = ReferenceId,
                          role = userRoles,
                          roleId = userRoleId,
                          token = new JwtSecurityTokenHandler().WriteToken(token),
                          expiration = token.ValidTo,
                          IsEmailConfirmed = true,
                          IsReportingto = ReportingTo,
                         // IsSafeGuardPerson = IsSafeGuardPerson

                      }); ;
                }
                else
                {
                    return StatusCode(500, "Account type not match");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}

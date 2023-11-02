using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.Login
{
    public class LoginService:ILoginService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        public LoginService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        
        //public async Task<ResponseModel> ExternalLogin(UserAuthModel uam)
        //{

        //    if (string.IsNullOrWhiteSpace(uam.Username))
        //    {
        //        return BadRequest(new { isError = true, msgError = "This email is not registered with us!" });
        //    }
        //    else if (string.IsNullOrWhiteSpace(uam.Key))
        //    {
        //        return BadRequest(new { isError = true, msgError = "Un-authorized User!" });
        //    }

        //    try
        //    {
        //        if (uam.AccountType == "MICROSOFT")
        //        {
        //            var FindUser = await _userManager.FindByEmailAsync(uam.Username);

        //            if (FindUser == null)
        //            {
        //                var registerUser = new IdentityUser
        //                {
        //                    Email = uam.Username,
        //                    UserName = uam.Username,
        //                    SecurityStamp = Guid.NewGuid().ToString(),
        //                    EmailConfirmed = true
        //                };
        //                var result = await _userManager.CreateAsync(registerUser);
        //                if (result.Succeeded)
        //                {
        //                    await _userManager.AddToRoleAsync(registerUser, "Developer");
        //                }
        //                // return Ok(new { Username = user.UserName });
        //            }
        //            // find user by email id 
        //            var user = await _userManager.FindByEmailAsync(uam.Username);
        //            List<Claim> claims = new List<Claim>();
        //            claims.Add(new Claim(ClaimTypes.Name, uam.Username));
        //            var claim = new[] {
        //                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
        //                    };

        //            // get user roles
        //            var userRoles = await _userManager.GetRolesAsync(user);
        //            IdentityRole userRoleId = new IdentityRole();
        //            if (userRoles.Count > 0)
        //            {
        //                userRoleId = await _roleManager.FindByNameAsync(userRoles[0]);
        //            }

        //            var signinKey = new SymmetricSecurityKey(
        //              Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

        //            int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

        //            var token = new JwtSecurityToken(
        //              issuer: _configuration["Jwt:Site"],
        //              audience: _configuration["Jwt:Site"],
        //              expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
        //              signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256Signature),
        //              claims: claims
        //            );

        //            int ReferenceId = 0;
        //            int ResourceId = 0;
        //            string ResourceName = "";
        //            string IsReportingto = "";
        //            string IsSafeGuardPerson = "";
        //            Resource objResource = _ResourceDAL.GetResourceByEmailId(user.UserName);
        //            if (objResource.ResourceId == 0)
        //            {
        //                //  ResourceName = user.UserName;
        //                return StatusCode(500, "Resource Not Available !!!");
        //            }

        //            else
        //                ResourceId = objResource.ResourceId;
        //            ResourceName = objResource.ResourceName;
        //            IsReportingto = objResource.IsReportingto;
        //            IsSafeGuardPerson = objResource.IsSafeGuardPerson;
        //            if (userRoles.Count > 0)
        //            {
        //                var Role = userRoles.ToArray();
        //                if (Role[0] == "Lead" || Role[0] == "Project Manager" || Role[0] == "Portfolio Manager")
        //                {
        //                    ReferenceId = objResource.ResourceId;
        //                }
        //            }
        //            return Ok(
        //              new
        //              {

        //                  id = user.Id,
        //                  username = user.UserName,
        //                  ResourceName = ResourceName,
        //                  ResourceId = ResourceId,
        //                  ReferenceId = ReferenceId,
        //                  role = userRoles,
        //                  roleId = userRoleId,
        //                  token = new JwtSecurityTokenHandler().WriteToken(token),
        //                  expiration = token.ValidTo,
        //                  IsEmailConfirmed = true,
        //                  IsReportingto = IsReportingto,
        //                  IsSafeGuardPerson = IsSafeGuardPerson

        //              }); ;
        //        }
        //        else
        //        {
        //            return StatusCode(500, "Account type not match");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}


    }
}

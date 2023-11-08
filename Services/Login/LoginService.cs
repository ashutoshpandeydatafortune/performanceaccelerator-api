using System;
using System.Text;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using DF_EvolutionAPI.Models.Response;

namespace DF_EvolutionAPI.Services.Login
{
    public class LoginService : ILoginService
    {
        static HttpClient msClient;

        private IResourceService _resourceService;
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginService(
            DFEvolutionDBContext dbContext,
            IResourceService resourceService,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager
        )
        {
            _dbcontext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;

            _resourceService = resourceService;

            msClient = new HttpClient();
            msClient.DefaultRequestHeaders.Accept.Clear();
            msClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<LoginResponse> ExternalLogin(UserAuthModel uam, IConfiguration configuration)
        {
            if (uam.AccountType == "MICROSOFT")
            {
                var existingUser = await _userManager.FindByEmailAsync(uam.Username);

                if (existingUser == null)
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
                }

                // find user by email id 
                var user = await _userManager.FindByEmailAsync(uam.Username);
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, uam.Username));
                var claim = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.UserName) };

                // get user roles
                var userRoles = await _userManager.GetRolesAsync(user);
                IdentityRole userRoleId = new IdentityRole();
                if (userRoles.Count > 0)
                {
                    userRoleId = await _roleManager.FindByNameAsync(userRoles[0]);
                }

                var signinKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]));

                int expiryInMinutes = Convert.ToInt32(configuration["Jwt:ExpiryInMinutes"]);

                var token = new JwtSecurityToken(
                  issuer: configuration["Jwt:Site"],
                  audience: configuration["Jwt:Site"],
                  expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                  signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256Signature),
                  claims: claims
                );

                int resourceId;
                int referenceId;
                int reportingTo;
                string resourceName;

                Resource resource = await _resourceService.GetResourceByEmailId(user.UserName);

                if (resource == null || resource.ResourceId == 0)
                {
                    throw new Exception("Resource Not Available !!!");
                }
                else
                {
                    resourceId = resource.ResourceId;
                    referenceId = resource.ResourceId;
                    resourceName = resource.ResourceName;
                    reportingTo = (int)resource.ReportingTo;
                }

                return new LoginResponse()
                {
                    Id = user.Id,
                    ResourceId = resourceId,
                    UserName = user.UserName,
                    ReferenceId = referenceId,
                    ResourceName = resourceName,
                    ReportingToId = reportingTo,

                    Roles = userRoles,
                    Role = userRoleId,
                    IsEmailConfirmed = true,
                    Expiration = token.ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                };
            }
            else
            {
                throw new Exception("Account type not match");
            }
        }
    }
}

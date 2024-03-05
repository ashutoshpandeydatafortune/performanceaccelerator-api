using System;
using System.Linq;
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
using DF_EvolutionAPI.Models.Response;
using System.Data;

namespace DF_EvolutionAPI.Services.Login
{
    public class LoginService : ILoginService
    {
        static HttpClient msClient;

        private IResourceService _resourceService;
        private readonly DFEvolutionDBContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
              

        public LoginService(
            DFEvolutionDBContext dbContext,
            IResourceService resourceService,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager 
          
        )
        {
            _dbContext = dbContext;
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

                var user = await _userManager.FindByEmailAsync(uam.Username);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, uam.Username)
                };

                // get user roles
                var userRoleNames = await _userManager.GetRolesAsync(user);
                
                List<Role> roles = new List<Role>();
                foreach (var userRoleName in userRoleNames)
                {
                    var identityRole = await _roleManager.FindByNameAsync(userRoleName);

                    var role = new Role();
                    role.RoleName = userRoleName;
                    role.RoleMappings = GetRoleMapping(identityRole);
                    roles.Add(role);
                }

                JwtSecurityToken token = GetAuthToken(configuration, claims);

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

                ResourceFunction resourceFunction = null;

                if (resource.FunctionId != null && resource.FunctionId.HasValue)
                {
                    resourceFunction = GetResourceFunction(resource.FunctionId.Value);
                }

                return new LoginResponse()
                {
                    Id = user.Id,
                    Roles = roles,
                    ResourceId = resourceId,
                    IsEmailConfirmed = true,
                    UserName = user.UserName,
                    ReferenceId = referenceId,
                    ResourceName = resourceName,
                    ReportingToId = reportingTo,
                    Expiration = token.ValidTo,
                    ResourceFunction = resourceFunction,
                    DesignationName = resource.DesignationName,
                    DesignationId = resource.DesignationId.Value,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                };
            }
            else
            {
                throw new Exception("Account type does not match");
            }
        }

        private List<RoleMapping> GetRoleMapping(IdentityRole role)
        {
            return (
                    from rm in _dbContext.PA_RoleMappings
                    where rm.RoleId == role.Id
                    select rm
                   ).ToList();
        }
        
        private ResourceFunction GetResourceFunction(int functionId)//Renamed parameter from resourceFunctionId to functionId to avoid confusion
        {
            return (
                    from rf in _dbContext.ResourceFunctions
                    where rf.ResourceFunctionId == functionId
                    select rf
                  ).FirstOrDefault();
        }

        private JwtSecurityToken GetAuthToken(IConfiguration configuration, List<Claim> claims)
        {
            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]));

            int expiryInMinutes = Convert.ToInt32(configuration["Jwt:ExpiryInMinutes"]);

            var token = new JwtSecurityToken(
              issuer: configuration["Jwt:Site"],
              audience: configuration["Jwt:Site"],
              expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
              signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256Signature),
              claims: claims
            );

            return token;
        }
    }
}

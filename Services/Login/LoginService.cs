using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
//using System.Reflection.Metadata;
using DF_EvolutionAPI.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
                var roleId = _dbContext.AspNetRoles.Where(roleid => roleid.Name == Constant.ROLE_NAME)
                            .FirstOrDefault() ?? throw new Exception("Role does not exist.");

                if (existingUser == null)
                {
                    var registerUser = RegisterUser(uam);
                    var result = await _userManager.CreateAsync(registerUser);

                    if (result.Succeeded)
                    {
                        await AddUserRole(registerUser.Id, roleId.Id);
                    }
                }
                else
                {
                    //Checking role for existing user.
                    var existingRole = _dbContext.AspNetUserRoles.Where(userRole => userRole.UserId == existingUser.Id
                                        && userRole.ApplicationName == Constant.APPLICATION_NAME)
                                        .FirstOrDefault();

                    if (existingRole == null)
                    {
                        await AddUserRole(existingUser.Id, roleId.Id);
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

                TechFunction TechFunction = null;

                if (resource.FunctionId != null && resource.FunctionId.HasValue)
                {
                    TechFunction = GetTechFunction(resource.FunctionId.Value);
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
                    ResourceFunction = TechFunction,
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
        //Method for register user
        private static IdentityUser RegisterUser(UserAuthModel uam)
        {
            var newUser = new IdentityUser
            {
                Email = uam.Username,
                UserName = uam.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true
            };
            return newUser;
        }

        //Method for adding the role for user
        private async Task AddUserRole(string userId, string roleId)
        {
            var newUserRole = new ApplicationUserRole
            {
                UserId = userId,
                RoleId = roleId,
                ApplicationName = Constant.APPLICATION_NAME
            };

            _dbContext.AspNetUserRoles.Add(newUserRole);
            await _dbContext.SaveChangesAsync();
        }
        private List<RoleMapping> GetRoleMapping(IdentityRole role)
        {
            return (
                    from rm in _dbContext.PA_RoleMappings
                    where rm.RoleId == role.Id
                    select rm
                   ).ToList();
        }

        private TechFunction GetTechFunction(int functionId)//Renamed parameter from resourceFunctionId to functionId to avoid confusion
        {
            return (
                    from rf in _dbContext.TechFunctions
                    where rf.FunctionId == functionId
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

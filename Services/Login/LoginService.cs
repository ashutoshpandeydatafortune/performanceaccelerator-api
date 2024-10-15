using System;
using System.Linq;
using System.Data;
using System.Text;
using System.Net.Http;
using DF_PA_API.Models;
using System.Security.Claims;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using DF_EvolutionAPI.Models.Response;
using Microsoft.Extensions.Configuration;

namespace DF_EvolutionAPI.Services.Login
{
    public class LoginService : ILoginService
    {
        static HttpClient msClient;

        private IResourceService _resourceService;
        private readonly DFEvolutionDBContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginService(
            DFEvolutionDBContext dbContext,
            IResourceService resourceService,
            UserManager<IdentityUser> userManager
        )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _resourceService = resourceService;

            msClient = new HttpClient();
            msClient.DefaultRequestHeaders.Accept.Clear();
            msClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Handles external login for Microsoft accounts.
        public async Task<LoginResponse> ExternalLogin(UserAuthModel uam, IConfiguration configuration)
        {
            // Check if the account type is Microsoft.
            if (uam.AccountType == "MICROSOFT")
            {
                // Find existing user by email.
                var existingUser = await _userManager.FindByEmailAsync(uam.Username);
                var roleId = _dbContext.RoleMasters.FirstOrDefault(r => r.IsDefault == true)
                             ?? throw new Exception("No default role is set in the system. Please ensure that at least one role is marked as default.");

                // If user does not exist, register a new user.
                if (existingUser == null)
                {
                    var registerUser = RegisterUser(uam);
                    var result = await _userManager.CreateAsync(registerUser);

                    // If registration succeeds, assign default role to the new user.
                    if (result.Succeeded)
                    {
                        await AddUserRole(registerUser.Id, roleId.RoleId);
                    }
                }
                else
                {
                    // If user exists, check if they already have a role assigned.
                    var existingRole = _dbContext.UserRoles.Where(userRole => userRole.UserId == existingUser.Id
                                       )
                        .FirstOrDefault();

                    // If no role is assigned, assign the default role.
                    if (existingRole == null)
                    {
                        await AddUserRole(existingUser.Id, roleId.RoleId);
                    }
                }

                // Find the user again after potential registration/role assignment.
                var user = await _userManager.FindByEmailAsync(uam.Username);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                // Create claims for the JWT token.
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, uam.Username)
                };

                // Retrieve user roles from the database.
                var userRoles = _dbContext.UserRoles.Where(ur => ur.UserId == user.Id)
                               .Join(_dbContext.RoleMasters, ur => ur.RoleId, rm => rm.RoleId,
                               (ur, rm) => new { ur, rm.RoleName })
                               .Select(r => r.RoleName).ToList();

                // Prepare roles and their mappings.
                List<RoleMaster> roles = new List<RoleMaster>();
                foreach (var userRoleName in userRoles)
                {
                    var role = _dbContext.RoleMasters.FirstOrDefault(r => r.RoleName == userRoleName);
                    if (role != null)
                    {
                        roles.Add(new RoleMaster
                        {
                            RoleName = userRoleName,
                            IsAdmin = role.IsAdmin,// Add this for  isAdmin flag
                            IsDefault = role.IsDefault,// Add this for isDefault flag
                            RoleMappings = GetRoleMapping(role)// Get associated role mappings.
                        });
                    }
                }

                // Generate JWT token.
                JwtSecurityToken token = GetAuthToken(configuration, claims);

                // Retrieve resource details associated with the user.
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

                // Retrieve technical function based on resource's function ID.
                var techFunction = resource.FunctionId.HasValue ? GetTechFunction(resource.FunctionId.Value) : null;

                // Return the login response containing user details and token.
                return new LoginResponse
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
                    ResourceFunction = techFunction,
                    DesignationName = resource.DesignationName,
                    DesignationId = resource.DesignationId.Value,
                    Token = new JwtSecurityTokenHandler().WriteToken(token), // Convert the token to string.
                };
            }
            else
            {
                throw new Exception("Account type does not match");
            }
        }

        // Helper method to create a new IdentityUser instance.
        private static IdentityUser RegisterUser(UserAuthModel uam)
        {
            return new IdentityUser
            {
                Email = uam.Username,
                UserName = uam.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true
            };
        }

        // Method to add a user role to the database.
        private async Task AddUserRole(string userId, string roleId)
        {
            var newUserRole = new UserRole
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                RoleId = roleId,
            };

            _dbContext.UserRoles.Add(newUserRole);
            await _dbContext.SaveChangesAsync();
        }

        // Method to retrieve role mappings associated with a role.
        private List<RoleMapping> GetRoleMapping(RoleMaster role)
        {
            return _dbContext.PA_RoleMappings
                .Where(rm => rm.RoleId == role.RoleId)
                .ToList();
        }

        // Method to retrieve a technical function based on its ID.
        private TechFunction GetTechFunction(int functionId)
        {
            return (
                    from techFunction in _dbContext.TechFunctions
                    where techFunction.FunctionId == functionId
                    select techFunction
                  ).FirstOrDefault();
        }

        // Method to generate a JWT token with specified claims.
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

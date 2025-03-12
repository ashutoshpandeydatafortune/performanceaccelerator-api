using System;
using System.Linq;
using System.Data;
using DF_PA_API.Models;
using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;



namespace DF_EvolutionAPI.Services
{
    // This class implements the ISettingsService interface for managing role mappings and permissions.
    public class SettingsService : ISettingsService
    {
        private readonly DFEvolutionDBContext _dbcontext; // Database context for accessing the database.
        
        // Constructor to initialize the database context.
        public SettingsService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;

        }

        // Retrieves all role mappings associated with a particular role ID.
        public async Task<List<RoleMapping>> GetPermissionsByRole(string roleId)
        {
            return await _dbcontext.PA_RoleMappings
                .Where(r => r.RoleId == roleId)
                .ToListAsync(); // Returns a list of role mappings for the specified role ID.
        }

        // Updates the permissions for a specific role mapping.
        public async Task<ResponseModel> UpdatePermissionByRole(RoleMapping roleMapping)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                // Find the existing role mapping using the provided RoleMappingId.
                var existingRoleMappings = _dbcontext.PA_RoleMappings.Find(roleMapping.RoleMappingId);

                // If the role mapping exists, update its properties.
                if (existingRoleMappings != null)
                {
                    existingRoleMappings.RoleId = roleMapping.RoleId;
                    existingRoleMappings.ModuleName = roleMapping.ModuleName;
                    existingRoleMappings.CanRead = roleMapping.CanRead;
                    existingRoleMappings.CanWrite = roleMapping.CanWrite;
                    existingRoleMappings.CanDelete = roleMapping.CanDelete;
                    existingRoleMappings.IsActive = (int)Status.IS_ACTIVE; // Set the role mapping as active.
                    existingRoleMappings.UpdateDate = DateTime.Now;
                    existingRoleMappings.UpdateBy = roleMapping.UpdateBy;// Set the user who updated the mapping.

                    await _dbcontext.SaveChangesAsync();// Save changes to the database.
                }

                model.IsSuccess = true;// Indicate that the update was successful.
                model.Messsage = "RoleMapping is updated successfully.";// Success message.

            }
            catch (Exception ex)
            {
                // Handle exceptions by setting the response model to indicate failure and providing the error message.
                model.IsSuccess = false;
                model.Messsage = ex.Message;
            }
            return model;// Return the response model.
        }

        // Adds or updates a list of role mappings.
        public async Task<ResponseModel> AddRoleMapping(List<RoleMapping> roleMappings)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                // Iterate over each role mapping to add or update.
                foreach (var roleMapping in roleMappings)
                {
                    // Remove existing role mappings with the same RoleId to ensure only one mapping exists per role.
                    var existingRoleMappings = _dbcontext.PA_RoleMappings
                        .Where(rm => rm.RoleId == roleMapping.RoleId)
                        .ToList();

                    _dbcontext.PA_RoleMappings.RemoveRange(existingRoleMappings);// Remove existing mappings.

                    // Create a new role mapping instance with the specified properties.
                    var newRoleMapping = new RoleMapping
                    {
                        RoleMappingId = roleMapping.RoleMappingId,
                        RoleId = roleMapping.RoleId,
                        ModuleName = roleMapping.ModuleName,
                        CanRead = roleMapping.CanRead,
                        CanWrite = roleMapping.CanWrite,
                        CanDelete = roleMapping.CanDelete,
                        IsActive = (int)Status.IS_ACTIVE,// Set the role mapping as active.
                        CreateDate = DateTime.Now,// Set the creation date.
                        CreateBy = roleMapping.CreateBy,// Set the user who created the mapping.
                    };

                    _dbcontext.PA_RoleMappings.Add(newRoleMapping);// Add the new role mapping to the database.
                }

                await _dbcontext.SaveChangesAsync();// Save all changes to the database.

                model.IsSuccess = true; // Indicate that the addition was successful.
                model.Messsage = "RoleMappings are updated successfully.";
            }

            catch (Exception ex)
            {
                // Handle exceptions by setting the response model to indicate failure and providing the error message.
                model.IsSuccess = false;
                model.Messsage = ex.Message;
            }
            return model;// Return the response model.
        }

        public async Task<List<RoleMaster>> GetAllRoles()
        {
            return await _dbcontext.RoleMasters.Where(r => r.IsActive == (int)Status.IS_ACTIVE)
                     .OrderBy(role => role.RoleName)
                    .ToListAsync();
        }

        //Retrieves a list of user resource roles where the resources and roles are active.
        public async Task<List<UserResourceRole>> GetAllUsersRole()
        {
            var query = from userRole in _dbcontext.UserRoles // Adjust based on your DbSet names
                        join user in _dbcontext.AspNetUsers on userRole.UserId equals user.Id
                        join resource in _dbcontext.Resources on user.Email equals resource.EmailId
                        where resource.IsActive == (int)Status.IS_ACTIVE && resource.StatusId == 8 //fecth the only active record by checking the statusid.
                        join role in _dbcontext.RoleMasters on userRole.RoleId equals role.RoleId
                        where role.IsActive == (int)Status.IS_ACTIVE
                        orderby resource.ResourceName
                        select new UserResourceRole
                        {
                            ResourceName = resource.ResourceName,
                            Email = user.Email,
                            RoleName = role.RoleName,
                            RoleId = role.RoleId
                        };

            var result =  await query.ToListAsync();
            return result;
            
        }

        public async Task<userEmail> GetAdminEmail()
        {
            var email = await (from roles in _dbcontext.RoleMasters
                               join userroles in _dbcontext.UserRoles on roles.RoleId equals userroles.RoleId
                               join users in _dbcontext.AspNetUsers on userroles.UserId equals users.Id
                               where roles.IsAdmin == true
                               select users.Email).FirstOrDefaultAsync();

            // Return an instance of userEmail with the retrieved email
            return new userEmail { Email = email }; // Returns null if no email is found
        }


        // This private method creates a new user role entry in the UserRoles table.
        public ResponseModel CreateOrUpdateUserRole(UserRoles userRoles)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                // Retrieve the user ID based on the provided email. 
                // If the user does not exist, throw an exception.
                var userId = _dbcontext.AspNetUsers.Where(user => user.Email == userRoles.Email).Select(user => user.Id)
                           .FirstOrDefault() ?? throw new Exception("The user does not exist");

               
                // Check if the user already has a role assigned.
                var existingUserRole = _dbcontext.UserRoles
                                    .Where(userRole => userRole.UserId == userId).FirstOrDefault() ?? throw new Exception("User does not have an assigned role.");

                // If a role exists, remove the existing user role from the database.
                _dbcontext.UserRoles.Remove(existingUserRole);
                
                // Create a new user role with the specified user ID and role ID.
                CreateNewUserRole(userId, userRoles.RoleId);

                _dbcontext.SaveChanges();

                model.IsSuccess = true;
                model.Messsage = "User role updated successfully.";
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "An error occurred: " + ex.Message;
            }

            return model;
        }

        private void CreateNewUserRole(string userId, string roleId)
        {
            var newUserRole = new UserRole
            {
                Id = Guid.NewGuid().ToString(),// Generate a new unique ID for the user role.
                UserId = userId,// Set the user ID.
                RoleId = roleId,// Set the role ID.             
            };
            // Add the new user role to the UserRoles DbSet.
            _dbcontext.UserRoles.Add(newUserRole);
        }
    }
}

using System;
using System.Linq;
using System.Data;
using DF_PA_API.Models;
using System.Data.Entity;
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

        // This method inserts or updates user roles based on the provided email and role name.
        public ResponseModel CreateOrUpdateUserRole(string emailId, string roleName)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                // Retrieve the user ID based on the provided email. 
                // If the user does not exist, throw an exception.
                var userId = _dbcontext.AspNetUsers.Where(user => user.Email == emailId).Select(user => user.Id)
                           .FirstOrDefault() ?? throw new Exception("The user does not exist");

                // Retrieve the role ID based on the provided role name.
                // If the role does not exist, throw an exception.
                var roleId = _dbcontext.RoleMasters.Where(role => role.RoleName == roleName).Select(role => role.RoleId)
                                            .FirstOrDefault() ?? throw new Exception("The role does not exist");
               
                // Check if the user already has a role assigned.
                var existingUserRole = _dbcontext.UserRoles
                                    .Where(userRole => userRole.UserId == userId ).FirstOrDefault();

                // If a role exists, remove the existing user role from the database.
                if (existingUserRole != null)
                {
                    _dbcontext.UserRoles.Remove(existingUserRole);
                }

                // Create a new user role with the specified user ID and role ID.
                CreateNewUserRole(userId, roleId);

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

        // This private method creates a new user role entry in the UserRoles table.
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

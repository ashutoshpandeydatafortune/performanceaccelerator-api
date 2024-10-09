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
    public class SettingsService : ISettingsService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public SettingsService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;

        }

        //Displays all the assign roles for particular roleid
        public async Task<List<RoleMapping>> GetPermissionsByRole(string roleId)
        {
            return await _dbcontext.PA_RoleMappings
                .Where(r => r.RoleId == roleId)
                .ToListAsync();
        }

        public async Task<ResponseModel> UpdatePermissionByRole(RoleMapping roleMapping)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var existingRoleMappings = _dbcontext.PA_RoleMappings.Find(roleMapping.RoleMappingId);

                if (existingRoleMappings != null)
                {
                    existingRoleMappings.RoleId = roleMapping.RoleId;
                    existingRoleMappings.ModuleName = roleMapping.ModuleName;
                    existingRoleMappings.CanRead = roleMapping.CanRead;
                    existingRoleMappings.CanWrite = roleMapping.CanWrite;
                    existingRoleMappings.CanDelete = roleMapping.CanDelete;
                    existingRoleMappings.IsActive = 1;
                    existingRoleMappings.UpdateDate = DateTime.Now;
                    existingRoleMappings.UpdateBy = roleMapping.UpdateBy;


                    await _dbcontext.SaveChangesAsync();
                }

                model.IsSuccess = true;
                model.Messsage = "RoleMapping is updated successfully.";

            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = ex.Message;
            }
            return model;
        }
        public async Task<ResponseModel> AddRoleMapping(List<RoleMapping> roleMappings)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                foreach (var roleMapping in roleMappings)
                {
                    // Remove existing role mappings with the same RoleId
                    var existingRoleMappings = _dbcontext.PA_RoleMappings
                        .Where(rm => rm.RoleId == roleMapping.RoleId)
                        .ToList();

                    _dbcontext.PA_RoleMappings.RemoveRange(existingRoleMappings);

                    // Add new role mapping
                    var newRoleMapping = new RoleMapping
                    {
                        RoleMappingId = roleMapping.RoleMappingId,
                        RoleId = roleMapping.RoleId,
                        ModuleName = roleMapping.ModuleName,
                        CanRead = roleMapping.CanRead,
                        CanWrite = roleMapping.CanWrite,
                        CanDelete = roleMapping.CanDelete,
                        IsActive = 1,
                        CreateDate = DateTime.Now,
                        CreateBy = roleMapping.CreateBy,
                    };

                    _dbcontext.PA_RoleMappings.Add(newRoleMapping);
                }

                await _dbcontext.SaveChangesAsync();

                model.IsSuccess = true;
                model.Messsage = "RoleMappings are updated successfully.";
            }

            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = ex.Message;
            }
            return model;
        }

        public async Task<List<RoleMaster>> GetAllRoles()
        {
            return await _dbcontext.RoleMasters.Where(r => r.IsActive == 1)
                     .OrderBy(role => role.RoleName)
                    .ToListAsync();
        }

        //Insert and update user roles.
        public ResponseModel CreateOrUpdateUserRole(string emailId, string roleName)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var userId = _dbcontext.AspNetUsers.Where(user => user.Email == emailId).Select(user => user.Id)
                           .FirstOrDefault() ?? throw new Exception("The user does not exist");

                //var roleId = _dbcontext.AspNetRoles.Where(role => role.Name == roleName).Select(role => role.Id)
                //            .FirstOrDefault() ?? throw new Exception("The role does not exist");

                var roleId = _dbcontext.RoleMasters.Where(role => role.RoleName == roleName).Select(role => role.RoleId)
                                            .FirstOrDefault() ?? throw new Exception("The role does not exist");

                var existingUserRole = _dbcontext.UserRoles
                                    .Where(userRole => userRole.UserId == userId ).FirstOrDefault();

                if (existingUserRole != null)
                {
                    _dbcontext.UserRoles.Remove(existingUserRole);
                }

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

        //private void CreateNewUserRole(string userId, string roleId, string applicationName)
        //{
        //    var newUserRole = new ApplicationUserRole
        //    {
        //        UserId = userId,
        //        RoleId = roleId,
        //        ApplicationName = applicationName
        //    };
        //    _dbcontext.AspNetUserRoles.Add(newUserRole);
        //}

        private void CreateNewUserRole(string userId, string roleId)
        {
            var newUserRole = new UserRole
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                RoleId = roleId,
                // ApplicationName = Constant.APPLICATION_NAME
            };
            _dbcontext.UserRoles.Add(newUserRole);
        }
    }
}

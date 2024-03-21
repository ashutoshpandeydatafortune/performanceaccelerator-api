﻿using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Models.Response;
using DF_EvolutionAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class SettingsService:ISettingsService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public SettingsService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;

        }

        //Displays all the assign roles for particular roleid
        public async Task<List<RoleMapping>> GetPermissionByRole(string roleId)
        {
            return  _dbcontext.PA_RoleMappings
                .Where(r => r.RoleId == roleId)
                .ToList();           
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
                        CreateDate = DateTime.Now
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



        public async Task<List<IdentityRole>> GetAllRoles()
        {
            return _dbcontext.AspNetRoles.ToList();
        }
        //Update or create roles for user.
        public ResponseModel CreateOrUpdateUserRole(string emialId, string roleName)
        {
            ResponseModel model = new ResponseModel();
            try { 
                    var roleId = _dbcontext.AspNetRoles.Where(role => role.Name == roleName).Select(role => role.Id).First();
                    var userId = _dbcontext.AspNetUser.Where(user => user.Email == emialId).Select(user => user.Id).First();
                    var existingUserRole = _dbcontext.AspNetUserRole.FirstOrDefault(userRole => userRole.UserId == userId);

                if (existingUserRole != null)
                {
                    existingUserRole.UserId = userId;
                    existingUserRole.RoleId = roleId;
                   // _dbcontext.AspNetUserRole.Update(existingUserRole); 
                }
                else
                {
                    var newUserRole = new IdentityUserRole<string>
                    {
                        UserId = userId,
                        RoleId = roleId
                    };
                    _dbcontext.AspNetUserRole.Add(newUserRole); // Add new user role
                }
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

       
    }
}

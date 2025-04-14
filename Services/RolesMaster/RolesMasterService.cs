using System;
using System.Linq;
using DF_EvolutionAPI;
using DF_PA_API.Models;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Services;

namespace DF_PA_API.Services.RolesMaster
{
    public class RolesMasterService : IRolesMasterService
    {
        private readonly DFEvolutionDBContext _dbContext;
        private readonly ILogger<RolesMasterService> _logger;

        public RolesMasterService(DFEvolutionDBContext dbContext, ILogger<RolesMasterService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseModel> CreateRoleMaster(RoleMaster roleMaster)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var existingRole = _dbContext.RoleMasters.FirstOrDefault(role => role.RoleName == roleMaster.RoleName && role.IsActive == (int)Status.IS_ACTIVE);

                if (existingRole == null)
                {
                    roleMaster.IsActive = (int)Status.IS_ACTIVE;
                    roleMaster.CreateDate = DateTime.UtcNow;

                    await _dbContext.RoleMasters.AddAsync(roleMaster);
                    await _dbContext.SaveChangesAsync();

                    model.Messsage = "Role created successfully.";
                    model.IsSuccess = true;
                }
                else
                {
                    model.Messsage = "Role already exists.";
                    model.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }

        public async Task<ResponseModel> UpdateRoleMaster(RoleMaster roleMaster)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var existingRole = _dbContext.RoleMasters.FirstOrDefault(role => role.RoleName == roleMaster.RoleName && role.IsActive == (int)Status.IS_ACTIVE);

                if (existingRole != null && existingRole.RoleId != roleMaster.RoleId)
                {
                    model.Messsage = "Role with the same name already exists.";
                    model.IsSuccess = false;
                    return model;
                }

                var roleToUpdate = await _dbContext.RoleMasters.FindAsync(roleMaster.RoleId);
                if (roleToUpdate != null)
                {
                    roleToUpdate.RoleName = roleMaster.RoleName;
                    roleToUpdate.Description = roleMaster.Description;
                    roleToUpdate.IsActive = (int)Status.IS_ACTIVE;
                    roleToUpdate.UpdateBy = roleMaster.UpdateBy;
                    roleToUpdate.UpdateDate = DateTime.UtcNow;
                    roleToUpdate.IsDefault = roleMaster.IsDefault;
                    roleToUpdate.IsAdmin = roleMaster.IsAdmin;

                    await _dbContext.SaveChangesAsync();

                    model.Messsage = "Role updated successfully.";
                    model.IsSuccess = true;
                }
                else
                {
                    model.Messsage = "Role not found.";
                    model.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }

        public async Task<List<RoleMaster>> GetAllRolesMaster()
        {
            return await _dbContext.RoleMasters
                .Where(role => role.IsActive == (int)Status.IS_ACTIVE)
                .OrderBy(role => role.CreateDate)
                .ToListAsync();
        }

        public async Task<ResponseModel> DeleteRoleMasterById(string id)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var roleToDelete = await _dbContext.RoleMasters.FindAsync(id);
                if (roleToDelete != null)
                {
                    roleToDelete.IsActive = (int)Status.IN_ACTIVE;
                    roleToDelete.UpdateDate = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();

                    model.IsSuccess = true;
                    model.Messsage = "Role deleted successfully.";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Role not found.";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }

            return model;
        }

    }
}


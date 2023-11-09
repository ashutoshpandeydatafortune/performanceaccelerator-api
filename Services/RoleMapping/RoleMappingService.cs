using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.RolesMapping
{
    public class RoleMappingService : IRoleMappingService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public RoleMappingService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<RoleMapping>> GetAllRoleMappingList()
        {
            return await _dbcontext.RoleMapping.Where(c => c.IsActive == 1).ToListAsync();
        }

        public async Task<RoleMapping> GetRoleMappingById(int roleMappingId)
        {
            RoleMapping roleMapping;

            try
            {
                roleMapping = await _dbcontext.FindAsync<RoleMapping>(roleMappingId);
            }
            catch (Exception)
            {
                throw;
            }
            
            return roleMapping;
        }
        public async Task<ResponseModel> CreateorUpdateRoleMapping(RoleMapping roleMappingModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                RoleMapping roleMapping = await GetRoleMappingById(roleMappingModel.Id);

                if (roleMapping != null)
                {
                    roleMapping.Email = roleMappingModel.Email;
                    roleMapping.UserId = roleMappingModel.UserId;
                    roleMapping.RoleId = roleMappingModel.RoleId;
                    roleMapping.IsActive = 1;
                    roleMapping.UpdateBy = 1;
                    roleMapping.UpdateDate = DateTime.Now;

                    _dbcontext.Update<RoleMapping>(roleMapping);
                    
                    model.Messsage = "Role Mapping Update Successfully";
                }
                else
                {
                    roleMappingModel.IsActive = 1;
                    roleMappingModel.CreateBy = 1;
                    roleMappingModel.UpdateBy = 1;
                    roleMappingModel.CreateDate = DateTime.Now;
                    roleMappingModel.UpdateDate = DateTime.Now;

                    _dbcontext.Add<RoleMapping>(roleMappingModel);
                    
                    model.Messsage = "Role Mapping Inserted Successfully";
                }

                _dbcontext.SaveChanges();
                
                model.IsSuccess = true;
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }
            return model;
        }

        public async Task<ResponseModel> DeleteRoleMapping(int roleMappingId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                RoleMapping roleMapping = await GetRoleMappingById(roleMappingId);

                if (roleMapping != null)
                {
                    roleMapping.IsDeleted = 1;

                    _dbcontext.Update(roleMapping);
                    _dbcontext.SaveChanges();
                    
                    model.IsSuccess = true;
                    model.Messsage = "Role Mapping Deleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Role Mapping Not Found";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }

            return model;
        }

        public async Task<List<RoleMapping>> GetRoleMappingByRoleId(int roleId)
        {
            var roleMappings = new List<RoleMapping>();
            try
            {
                roleMappings = await (
                    from rm in _dbcontext.RoleMapping.Where(x => x.RoleId == roleId)
                    select new RoleMapping
                    {
                        Id = rm.Id,
                        Email = rm.Email,
                        UserId = rm.UserId,
                        RoleId = rm.RoleId,
                        CreateBy = rm.CreateBy,
                        UpdateBy = rm.UpdateBy,
                        CreateDate = rm.CreateDate,
                        UpdateDate = rm.UpdateDate,
                        IsActive = rm.IsActive,
                    }).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return roleMappings;
        }

        public async Task<List<RoleMapping>> GetRoleMappingByUserId(int userId)
        {
            var roleMappings = new List<RoleMapping>();

            try
            {
                roleMappings = await (
                    from rm in _dbcontext.RoleMapping.Where(x => x.UserId == userId)
                    select new RoleMapping
                    {
                        Id = rm.Id,
                        Email = rm.Email,
                        UserId = rm.UserId,
                        RoleId = rm.RoleId,
                        CreateBy = rm.CreateBy,
                        UpdateBy = rm.UpdateBy,
                        CreateDate = rm.CreateDate,
                        UpdateDate = rm.UpdateDate,
                        IsActive = rm.IsActive,
                    }).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return roleMappings;
        }
    }
}

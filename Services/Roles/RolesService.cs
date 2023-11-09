using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class RolesService : IRolesService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public RolesService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<Roles>> GetAllRoleList()
        {
            return await _dbcontext.Roles.ToListAsync();
        }

        public async Task<Roles> GetRoleById(int roleId)
        {
            Roles roles;

            try
            {
                roles = await _dbcontext.FindAsync<Roles>(roleId);
            }
            catch (Exception)
            {
                throw;
            }

            return roles;
        }

        public async Task<ResponseModel> CreateorUpdateRole(Roles rolesModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                Roles role = await GetRoleById(rolesModel.Id);

                if (role != null)
                {
                    role.RoleName = rolesModel.RoleName;
                    role.Description = rolesModel.Description;
                    role.IsActive = 1;
                    role.UpdateBy = 1;
                    role.UpdateDate = DateTime.Now;

                    _dbcontext.Update(role);
                    
                    model.Messsage = "Role Updated Successfully";
                }
                else
                {
                    rolesModel.IsActive = 1;
                    rolesModel.CreateBy = 1;
                    rolesModel.UpdateBy = 1;
                    rolesModel.CreateDate = DateTime.Now;
                    rolesModel.UpdateDate = DateTime.Now;

                    _dbcontext.Add(rolesModel);
                    
                    model.Messsage = "Role Inserted Successfully";
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

        public async Task<ResponseModel> DeleteRole(int roleId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                Roles role = await GetRoleById(roleId);

                if (role != null)
                {
                    role.IsDeleted = 1;

                    _dbcontext.Update<Roles>(role);
                    _dbcontext.SaveChanges();
                    
                    model.IsSuccess = true;
                    model.Messsage = "Role Deleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Role Not Found";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }

            return model;
        }
    }
}

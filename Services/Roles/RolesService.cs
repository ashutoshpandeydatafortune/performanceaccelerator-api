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
            Roles quarterdetails;
            try
            {
                quarterdetails = await _dbcontext.FindAsync<Roles>(roleId);
            }
            catch (Exception)
            {
                throw;
            }
            return quarterdetails;
        }

        public async Task<ResponseModel> CreateorUpdateRole(Roles rolesModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                Roles _temp = await GetRoleById(rolesModel.Id);
                if (_temp != null)
                {
                    _temp.RoleName = rolesModel.RoleName;
                    _temp.Description = rolesModel.Description;
                    _temp.IsActive = 1;
                    _temp.UpdateBy = 1;
                    _temp.UpdateDate = DateTime.Now;
                    _dbcontext.Update<Roles>(_temp);
                    model.Messsage = "Role Updated Successfully";
                }
                else
                {
                    rolesModel.IsActive = 1;
                    rolesModel.CreateBy = 1;
                    rolesModel.UpdateBy = 1;
                    rolesModel.CreateDate = DateTime.Now;
                    rolesModel.UpdateDate = DateTime.Now;
                    _dbcontext.Add<Roles>(rolesModel);
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
                Roles _temp = await GetRoleById(roleId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<Roles>(_temp);
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

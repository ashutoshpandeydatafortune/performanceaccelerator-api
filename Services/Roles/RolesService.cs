using System;
using System.Linq;
using DF_PA_API.Models;
using DF_EvolutionAPI.Models;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;

namespace DF_EvolutionAPI.Services
{
    public class RolesService : IRolesService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public RolesService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public List<Role> GetAllRoleList()
        {
            return _dbcontext.Roles.ToList();
        }

        public Role GetRoleById(int roleId)
        {
            Role role;

            try
            {
                role = _dbcontext.Find<Role>(roleId);
            }
            catch (Exception)
            {
                throw;
            }

            return role;
        }

        public ResponseModel CreateorUpdateRole(Role rolesModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                Role role = GetRoleById(rolesModel.RoleId);

                if (role != null)
                {
                    role.RoleName = rolesModel.RoleName;
                    role.IsActive = (int)Status.IS_ACTIVE;

                    _dbcontext.Update(role);
                    
                    model.Messsage = "Role Updated Successfully";
                }
                else
                {
                    rolesModel.IsActive = (int)Status.IS_ACTIVE;

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

        public ResponseModel DeleteRole(int roleId)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                Role role = GetRoleById(roleId);

                if (role != null)
                {
                    role.IsActive = (int)Status.IN_ACTIVE;

                    _dbcontext.Update<Role>(role);
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

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services
{
    public class KRAWeightageService: IKRAWeightageService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        public KRAWeightageService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<KRAWeightage>> GetAllKRAWeightageList()
        {
            //return await _dbcontext.KRAWeightages.Where(c => c.IsActive == 1).ToListAsync();
            return await _dbcontext.KRAWeightages.ToListAsync();
        }

        public async Task<KRAWeightage> GetKRAWeightageDetailsById(int weightageId)
        {
            KRAWeightage weightage;
            try
            {
                weightage = await _dbcontext.FindAsync<KRAWeightage>(weightageId);
            }
            catch (Exception)
            {
                throw;
            }
            return weightage;
        }

        public async Task<ResponseModel> CreateorUpdateKRAWeightage(KRAWeightage weightageModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                KRAWeightage _temp = await GetKRAWeightageDetailsById(weightageModel.Id);
                if (_temp != null)
                {
                    _temp.Name = weightageModel.Name;
                    _temp.DisplayName = weightageModel.DisplayName;
                    _temp.Description = weightageModel.Description;
                    _temp.IsActive = 1;
                    _temp.UpdateBy = 1;
                    _temp.UpdateDate = DateTime.Now;
                    _dbcontext.Update(_temp);
                    model.Messsage = "KRA Weightage Update Successfully";
                }
                else
                {
                    weightageModel.IsActive = 1;
                    weightageModel.CreateBy = 1;
                    weightageModel.UpdateBy = 1;
                    weightageModel.CreateDate = DateTime.Now;
                    weightageModel.UpdateDate = DateTime.Now;
                    _dbcontext.Add<KRAWeightage>(weightageModel);
                    model.Messsage = "KRA Weightage Inserted Successfully";
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

        public async Task<ResponseModel> DeleteKRAWeightage(int weightageId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                KRAWeightage _temp = await GetKRAWeightageDetailsById(weightageId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<KRAWeightage>(_temp);
                    _dbcontext.SaveChanges();
                    model.IsSuccess = true;
                    model.Messsage = "KRA Weightage Deleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "KRA Weightage Not Found";
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

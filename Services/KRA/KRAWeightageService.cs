using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                KRAWeightage kraWeightage = await GetKRAWeightageDetailsById(weightageModel.Id);

                if (kraWeightage != null)
                {
                    kraWeightage.Name = weightageModel.Name;
                    kraWeightage.DisplayName = weightageModel.DisplayName;
                    kraWeightage.Description = weightageModel.Description;
                    kraWeightage.IsActive = 1;
                    kraWeightage.UpdateBy = 1;
                    kraWeightage.UpdateDate = DateTime.Now;

                    _dbcontext.Update(kraWeightage);

                    model.Messsage = "KRA Weightage Update Successfully";
                }
                else
                {
                    weightageModel.IsActive = 1;
                    weightageModel.CreateBy = 1;
                    weightageModel.UpdateBy = 1;
                    weightageModel.CreateDate = DateTime.Now;
                    weightageModel.UpdateDate = DateTime.Now;

                    _dbcontext.Add(weightageModel);

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
                KRAWeightage kraWeightage = await GetKRAWeightageDetailsById(weightageId);
            
                if (kraWeightage != null)
                {
                    kraWeightage.IsDeleted = 1;
                    _dbcontext.Update(kraWeightage);

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

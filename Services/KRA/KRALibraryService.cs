using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.KRA
{
    public class KRALibraryService : IKRALibraryService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        public KRALibraryService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<KRALibrary>> GetAllKRALibraryList()
        {
            return await _dbcontext.KRALibrary.Where(x => x.IsActive == 1).ToListAsync();
        }

        public async Task<KRALibrary> GetKRALibraryById(int kraLibraryId)
        {
            KRALibrary kralibrary;
            try
            {
                kralibrary = await _dbcontext.FindAsync<KRALibrary>(kraLibraryId);
            }
            catch (Exception)
            {
                throw;
            }
            return kralibrary;
        }

        public async Task<ResponseModel> CreateorUpdateKRALibrary(KRALibrary kraLibraryModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                KRALibrary _temp = await GetKRALibraryById(kraLibraryModel.Id);
                if (_temp != null)
                {
                    _temp.Name = kraLibraryModel.Name;
                    _temp.DisplayName = kraLibraryModel.DisplayName;
                    _temp.Description = kraLibraryModel.Description;
                    _temp.Entity = kraLibraryModel.Entity;
                    _temp.Entity2 = kraLibraryModel.Entity2;
                    _temp.ApprovedBy = kraLibraryModel.ApprovedBy;
                    _temp.RejectedBy = kraLibraryModel.RejectedBy;
                    _temp.Reason = "test";
                    _temp.Comment = "test";
                    _temp.IsSpecial = kraLibraryModel.IsSpecial;
                    _temp.IsDefault = kraLibraryModel.IsDefault;
                    _temp.WeightageId = kraLibraryModel.WeightageId;
                    _temp.IsActive = kraLibraryModel.IsActive;
                    _temp.UpdateBy = 1;
                    _temp.UpdateDate = DateTime.Now;
                    _temp.Weightage = kraLibraryModel.Weightage;
                    _dbcontext.Update<KRALibrary>(_temp);
                    model.Messsage = "KRA Library Update Successfully";
                }
                else
                {
                    kraLibraryModel.DisplayName = kraLibraryModel.Name;
                    kraLibraryModel.Description=kraLibraryModel.Description;
                    kraLibraryModel.Entity = 1;
                    kraLibraryModel.Entity2 = 1;
                    kraLibraryModel.ApprovedBy = 23;
                    kraLibraryModel.RejectedBy = 23;
                    kraLibraryModel.Reason = "test ";
                    kraLibraryModel.Comment = "test ";
                    kraLibraryModel.IsSpecial = kraLibraryModel.IsSpecial;
                    kraLibraryModel.IsDefault = 1;
                    kraLibraryModel.IsActive = 1;
                    kraLibraryModel.CreateBy = 1;
                    kraLibraryModel.UpdateBy = 1;
                    kraLibraryModel.CreateDate = DateTime.Now;
                    kraLibraryModel.UpdateDate = DateTime.Now;
                    _dbcontext.Add(kraLibraryModel);
                    model.Messsage = "KRA Library Inserted Successfully";
                }
                 _dbcontext.SaveChanges();
                model.Id = kraLibraryModel.Id;
                model.IsSuccess = true;
               
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }
            return model;
        }

        public async Task<ResponseModel> DeleteKRALibrary(int kraLibraryId)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                KRALibrary _temp = await GetKRALibraryById(kraLibraryId);
                if (_temp != null)
                {
                    _temp.IsDeleted = 1;
                    _dbcontext.Update<KRALibrary>(_temp);
                    _dbcontext.SaveChanges();
                    model.IsSuccess = true;
                    model.Messsage = "KRA Library Deleted Successfully";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "KRA Library Not Found";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }
            return model;
        }


        public async Task<KRAWeightage> GetKRAWeightageDetailsByKRALibraryId(int kraLibraryId)
        {
            ResponseModel model = new ResponseModel();
            KRAWeightage kraweightage = new KRAWeightage();
            try
            {
                KRALibrary kraLibrary = await GetKRALibraryById(kraLibraryId);
                kraweightage = await (from kw in _dbcontext.KRAWeightages
                                          where kw.Id == kraLibrary.WeightageId && kw.IsActive == 1
                                          select kw).FirstAsync();
                model.IsSuccess = false;
                if (kraweightage == null)
                {
                    model.Messsage = "KRA Weightage Does Not Exist";
                    return kraweightage;
                }
                model.IsSuccess = true;
                model.Messsage = "KRA Weightage Found Successfully";
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }
            return kraweightage;
        }


        public async Task<List<KRALibrary>> GetKRADetailsByWeightageId(int weightageId)
        {
            ResponseModel model = new ResponseModel();
            var kraDetails = new List<KRALibrary>();
            try
            {
                kraDetails = await (from kd in _dbcontext.KRALibrary.Where(x => x.WeightageId == weightageId)
                                    select new KRALibrary
                                    {
                                        Id = kd.Id,
                                        Name = kd.Name,
                                        DisplayName = kd.DisplayName,
                                        Description = kd.Description,
                                        Entity = kd.Entity,
                                        Entity2 = kd.Entity2,
                                        ApprovedBy = kd.ApprovedBy,
                                        RejectedBy = kd.RejectedBy,
                                        Reason = kd.Reason,
                                        Comment = kd.Comment,
                                        IsActive = kd.IsActive,
                                        IsDeleted = kd.IsDeleted,
                                        IsSpecial = kd.IsSpecial,
                                        WeightageId = kd.WeightageId,
                                        CreateBy = kd.CreateBy,
                                        UpdateBy = kd.UpdateBy,
                                        CreateDate = kd.CreateDate,
                                        UpdateDate = kd.UpdateDate,
                                        Weightage = kd.Weightage,

                                    }).ToListAsync();
                model.IsSuccess = false;

                if (kraDetails == null ||  kraDetails.Count == 0)
                {
                    model.Messsage = "KRA Details With " + weightageId + " Does Not Exist";  
                    return kraDetails;
                }
                model.IsSuccess = true;
                model.Messsage = "KRA Details With " + weightageId + " Found";
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                model.Messsage = "Error : " + ex.Message;
            }
            return kraDetails;
        }
    }
}


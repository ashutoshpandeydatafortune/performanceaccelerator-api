﻿using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DF_EvolutionAPI.Services.KRA
{
    public class KRALibraryService : IKRALibraryService
    {
        private readonly DFEvolutionDBContext _dbcontext;

        public KRALibraryService(DFEvolutionDBContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<List<KRAList>> GetAllKRALibraryList(int? isNotSpecial)
        {
            //Checked isSpecial condition for displaying kras list.
            if (isNotSpecial == null || isNotSpecial == 0)
            {
                //return await _dbcontext.KRALibrary.Where(x => x.IsActive == 1).ToListAsync();
                var query = from kraLibrary in _dbcontext.KRALibrary
                            join function in _dbcontext.TechFunctions
                            on kraLibrary.FunctionId equals function.FunctionId into kraTechGroup
                            from techFunc in kraTechGroup.DefaultIfEmpty()
                            where kraLibrary.IsActive == (int)Status.IS_ACTIVE && (kraLibrary.FunctionId == null || techFunc.FunctionId != null)
                            select new KRAList
                            {
                                Id = kraLibrary.Id,
                                Name = kraLibrary.Name,
                                DisplayName = kraLibrary.DisplayName,
                                Description = kraLibrary.Description,
                                Weightage = kraLibrary.Weightage,
                                IsDescriptionRequired = kraLibrary.IsDescriptionRequired,
                                MinimumRatingForDescription = kraLibrary.MinimumRatingForDescription,
                                FunctionName = techFunc != null ? techFunc.FunctionName : null,
                                FunctionId = kraLibrary.FunctionId?? null,
                                IsSpecial = kraLibrary.IsSpecial,                                
                            };

                return await query.ToListAsync();
            }
            else
            {
                //return await _dbcontext.KRALibrary.Where(x => x.IsActive == 1 && x.IsSpecial != 1).ToListAsync();
                var query = from kraLibrary in _dbcontext.KRALibrary
                            join function in _dbcontext.TechFunctions
                            on kraLibrary.FunctionId equals function.FunctionId
                            where kraLibrary.IsActive == (int)Status.IS_ACTIVE && kraLibrary.IsSpecial != 1
                            select new KRAList
                            {
                                Id= kraLibrary.Id,
                                Name = kraLibrary.Name,
                                DisplayName = kraLibrary.DisplayName,
                                Description = kraLibrary.Description,
                                Weightage = kraLibrary.Weightage,
                                IsDescriptionRequired = kraLibrary.IsDescriptionRequired,
                                MinimumRatingForDescription = kraLibrary.MinimumRatingForDescription,
                                FunctionId = kraLibrary.FunctionId?? null,
                                FunctionName = function.FunctionName != null ? function.FunctionName: null,
                                IsSpecial = kraLibrary.IsSpecial,
                            };

                return await query.ToListAsync();
            }

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
                // Check if a KRA with the same Name and FunctionId already exists for Update (excluding the current KRA being updated)
                var existingKRAUpdate = _dbcontext.KRALibrary.FirstOrDefault(k => k.Name == kraLibraryModel.Name && k.FunctionId == kraLibraryModel.FunctionId && k.Id != kraLibraryModel.Id && k.IsActive == (int)Status.IS_ACTIVE);
                // Check if a KRA with the same Name and FunctionId already exists for Create
                var existingKRA = _dbcontext.KRALibrary.FirstOrDefault(k => k.Name == kraLibraryModel.Name && k.FunctionId == kraLibraryModel.FunctionId && k.IsActive == (int)Status.IS_ACTIVE);

                KRALibrary kraLibrary = await GetKRALibraryById(kraLibraryModel.Id);
                if (kraLibrary != null)
                {
                    if (existingKRAUpdate != null)
                    {
                        model.IsSuccess = false;
                        model.Messsage = "KRA Library with the same name already exists.";
                        return model;
                    }

                    kraLibrary.Name = kraLibraryModel.Name;
                    kraLibrary.DisplayName = kraLibraryModel.Name;
                    kraLibrary.Description = kraLibraryModel.Description;
                    kraLibrary.Entity = kraLibraryModel.Entity;
                    kraLibrary.Entity2 = kraLibraryModel.Entity2;
                    kraLibrary.ApprovedBy = kraLibraryModel.ApprovedBy;
                    kraLibrary.RejectedBy = kraLibraryModel.RejectedBy;
                    kraLibrary.Reason = "test";
                    kraLibrary.Comment = "test";
                    kraLibrary.IsSpecial = kraLibraryModel.IsSpecial;
                    kraLibrary.IsDefault = kraLibraryModel.IsDefault;
                    kraLibrary.WeightageId = kraLibraryModel.WeightageId;
                    kraLibrary.IsActive = (int)Status.IS_ACTIVE;
                    kraLibrary.UpdateBy = kraLibraryModel.UpdateBy; ;
                    kraLibrary.UpdateDate = DateTime.Now;
                    kraLibrary.Weightage = kraLibraryModel.Weightage;
                    kraLibrary.IsDescriptionRequired = kraLibraryModel.IsDescriptionRequired;
                    kraLibrary.MinimumRatingForDescription = kraLibraryModel.MinimumRatingForDescription;
                    kraLibrary.FunctionId = kraLibraryModel.FunctionId;

                    _dbcontext.Update<KRALibrary>(kraLibrary);

                    model.Messsage = "KRA Library Update Successfully";
                }
                else
                {
                    if (existingKRA != null)
                    {
                        model.IsSuccess = false;
                        model.Messsage = "KRA Library with the same name already exists.";
                        return model;
                    }
                    // Check if a KRALibrary record with the same name already exists

                    kraLibraryModel.Name = kraLibraryModel.Name;
                    kraLibraryModel.DisplayName = kraLibraryModel.Name;
                    kraLibraryModel.Description = kraLibraryModel.Description;
                    kraLibraryModel.Entity = 1;
                    kraLibraryModel.Entity2 = 1;
                    kraLibraryModel.ApprovedBy = 23;
                    kraLibraryModel.RejectedBy = 23;
                    kraLibraryModel.Reason = "test ";
                    kraLibraryModel.Comment = "test ";
                    kraLibraryModel.IsSpecial = kraLibraryModel.IsSpecial;
                    kraLibraryModel.IsDefault = 1;
                    kraLibraryModel.IsActive = (int)Status.IS_ACTIVE;
                    kraLibraryModel.CreateBy = kraLibraryModel.CreateBy;
                    kraLibraryModel.CreateDate = DateTime.Now;
                    kraLibraryModel.IsDescriptionRequired = kraLibraryModel.IsDescriptionRequired;
                    kraLibraryModel.MinimumRatingForDescription = kraLibraryModel.MinimumRatingForDescription;
                    kraLibraryModel.FunctionId = kraLibraryModel.FunctionId;



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
                KRALibrary kraLibrary = await GetKRALibraryById(kraLibraryId);

                if (kraLibrary != null)
                {
                    kraLibrary.IsDeleted = 1;
                    kraLibrary.IsActive = (int)Status.IN_ACTIVE;

                    _dbcontext.Update(kraLibrary);
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
                kraweightage = await (
                    from kw in _dbcontext.KRAWeightages
                    where kw.Id == kraLibrary.WeightageId && kw.IsActive == (int)Status.IS_ACTIVE
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
                kraDetails = await (
                    from kd in _dbcontext.KRALibrary.Where(x => x.WeightageId == weightageId)
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
                        IsDescriptionRequired = kd.IsDescriptionRequired,
                        MinimumRatingForDescription = kd.MinimumRatingForDescription,

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

        // Get the all kras functionwise.
        public async Task<List<KRAList>> GetAllKRAsByFunction(int functionId)
        {      
                var query = from kraLibrary in _dbcontext.KRALibrary
                            join function in _dbcontext.TechFunctions
                            on kraLibrary.FunctionId equals function.FunctionId
                            where kraLibrary.IsActive == (int)Status.IS_ACTIVE && kraLibrary.FunctionId == functionId
                            select new KRAList
                            {
                                Id = kraLibrary.Id,
                                Name = kraLibrary.Name,
                                DisplayName = kraLibrary.DisplayName,
                                Description = kraLibrary.Description,
                                Weightage = kraLibrary.Weightage,
                                IsDescriptionRequired = kraLibrary.IsDescriptionRequired,
                                MinimumRatingForDescription = kraLibrary.MinimumRatingForDescription,
                                FunctionId = kraLibrary.FunctionId ?? null,
                                FunctionName = function.FunctionName ?? null
                            };

                return await query.ToListAsync();
        }
        
    }
}


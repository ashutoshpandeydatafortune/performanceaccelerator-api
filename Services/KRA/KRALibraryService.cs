using System;
using System.Linq;
using DF_PA_API.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Models.Response;

namespace DF_EvolutionAPI.Services.KRA
{
    public class KRALibraryService : IKRALibraryService
    {
        private readonly DFEvolutionDBContext _dbcontext;
        private readonly ILogger<KRALibraryService> _logger;

        public KRALibraryService(DFEvolutionDBContext dbContext, ILogger<KRALibraryService> logger)
        {
            _dbcontext = dbContext;
            _logger = logger;
        }

        public async Task<List<KRAList>> GetAllKRALibraryList(int? isNotSpecial)
        {
            _logger.LogInformation("Processing started in Class: {Class}, Method :{Method}", nameof(KRAList), nameof(GetAllKRALibraryList));
            _logger.LogInformation("Fetching KRA list. IsNotSpecial filter: {IsNotSpecial}", isNotSpecial);

            try
            { 
            //Checked isSpecial condition for displaying kras list.
            if (isNotSpecial == null || isNotSpecial == 0)
            {
                    _logger.LogInformation("Fetching all KRA records including special ones.");

                    //return await _dbcontext.KRALibrary.Where(x => x.IsActive == 1).ToListAsync();
                    var query = from kraLibrary in _dbcontext.KRALibrary
                            join function in _dbcontext.TechFunctions
                            on kraLibrary.FunctionId equals function.FunctionId into kraTechGroup
                            from techFunc in kraTechGroup.DefaultIfEmpty()
                            join businessUnit in _dbcontext.BusinessUnits
                            on kraLibrary.BusinessUnitId equals businessUnit.BusinessUnitId into businessUnitGroup
                            from bu in businessUnitGroup.DefaultIfEmpty()
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
                                BusinessUnitId = kraLibrary.BusinessUnitId?? null,
                                BusinessUnitName = bu != null ? bu.BusinessUnitName : null,
                            };

                return await query.OrderBy(kra => kra.Name).ToListAsync();
            }
            else
            {
                //return await _dbcontext.KRALibrary.Where(x => x.IsActive == 1 && x.IsSpecial != 1).ToListAsync();
                var query = from kraLibrary in _dbcontext.KRALibrary
                            join function in _dbcontext.TechFunctions
                            on kraLibrary.FunctionId equals function.FunctionId
                            join businessUnit in _dbcontext.BusinessUnits
                            on kraLibrary.BusinessUnitId equals businessUnit.BusinessUnitId
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
                                BusinessUnitId = kraLibrary.BusinessUnitId,
                                BusinessUnitName = businessUnit != null ? businessUnit.BusinessUnitName : null,
                            };

                    var result = await query.OrderBy(kra => kra.Name).ToListAsync();
                    _logger.LogInformation("Retrieved {Count} KRA records (excluding special).", result.Count);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                throw;
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
                var existingKRAUpdate = _dbcontext.KRALibrary.FirstOrDefault(k => k.Name == kraLibraryModel.Name && k.FunctionId == kraLibraryModel.FunctionId && k.BusinessUnitId == kraLibraryModel.BusinessUnitId && k.Id != kraLibraryModel.Id && k.IsActive == (int)Status.IS_ACTIVE);
                // Check if a KRA with the same Name, FunctionId and BusinessunitId already exists for Create
                var existingKRA = _dbcontext.KRALibrary.FirstOrDefault(k => k.Name == kraLibraryModel.Name
                && k.FunctionId == kraLibraryModel.FunctionId && k.BusinessUnitId == kraLibraryModel.BusinessUnitId && k.IsActive == (int)Status.IS_ACTIVE && k.IsSpecial != 1);
                // Check if a  Special KRA with the same Name already exists for Create
                var IsSpecialKra = _dbcontext.KRALibrary.FirstOrDefault(k => k.Name == kraLibraryModel.Name
                && k.FunctionId == kraLibraryModel.FunctionId && k.BusinessUnitId == kraLibraryModel.BusinessUnitId && k.IsActive == (int)Status.IS_ACTIVE && k.IsSpecial == 1);

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
                    kraLibrary.BusinessUnitId = kraLibraryModel.BusinessUnitId;

                    _dbcontext.Update<KRALibrary>(kraLibrary);

                    model.Messsage = "KRA Library Update Successfully";
                }
                else
                {
                    if (IsSpecialKra != null)
                    {
                        model.IsSuccess = false;
                        model.Messsage = " Special KRA Library with the same name already exists.";
                        return model;
                    }

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
                    kraLibraryModel.BusinessUnitId = kraLibraryModel.BusinessUnitId;

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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
                            orderby kraLibrary.Name // OrderBy KraName ascending.
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

        // Get the all kras businessUnitWise.
        public async Task<List<KRAList>> GetAllKRAsByBusinessUnit(int businessUnitId)
        {
            var query = from kraLibrary in _dbcontext.KRALibrary
                        join function in _dbcontext.TechFunctions
                        on kraLibrary.FunctionId equals function.FunctionId
                        where kraLibrary.IsActive == (int)Status.IS_ACTIVE && kraLibrary.BusinessUnitId == businessUnitId
                        orderby kraLibrary.Name // OrderBy KraName ascending.
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

        // Gets the KRAs assigned to resources based on KRA ID and Function ID.
        public async Task<List<AssignedUserKraDetail>> GetAssignedUserKras(int kraId, int functionId)
        {
            var result = await (from uk in _dbcontext.UserKRA
                                join k in _dbcontext.KRALibrary on uk.KRAId equals k.Id
                                join r in _dbcontext.Resources on uk.UserId equals r.ResourceId
                                join f in _dbcontext.TechFunctions on r.FunctionId equals f.FunctionId
                                where uk.IsApproved == 0
                                      && uk.IsActive == 1
                                      && r.IsActive == 1
                                      && k.IsActive == 1
                                      && k.Id == kraId
                                      && r.FunctionId == functionId
                                select new AssignedUserKraDetail
                                {
                                    Id = k.Id,
                                    KraName = k.Name,
                                    QuarterId = uk.QuarterId,
                                    ResourceName = r.ResourceName,
                                    FunctionId = r.FunctionId

                                }).ToListAsync();

            return result;
        }

    }
}


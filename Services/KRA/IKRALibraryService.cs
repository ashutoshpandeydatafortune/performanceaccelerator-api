﻿using DF_EvolutionAPI.Models;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;

namespace DF_EvolutionAPI.Services.KRA
{
    public interface IKRALibraryService
    {       
        public Task<KRALibrary> GetKRALibraryById(int kraLibraryId);
        public Task<ResponseModel> DeleteKRALibrary(int kraLibraryId);
        public Task<List<KRAList>> GetAllKRAsByFunction(int functionId);
        public Task<List<KRAList>> GetAllKRALibraryList(int? isNotSpecial);
        public Task<List<KRALibrary>> GetKRADetailsByWeightageId(int weightageId);
        public Task<ResponseModel> CreateorUpdateKRALibrary(KRALibrary kraLibraryModel);
        public Task<KRAWeightage> GetKRAWeightageDetailsByKRALibraryId(int kraLibraryId);
        public Task<List<AssignedUserKraDetail>> GetAssignedUserKras(int kraId, int functionId);
        public Task<List<KRAList>> GetAllKRAsByBusinessUnit(int businessUnitId);
    }
}

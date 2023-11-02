﻿using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Services.KRA
{
    public interface IKRALibraryService
    {
        public Task<List<KRALibrary>> GetAllKRALibraryList();

        public Task<KRALibrary> GetKRALibraryById(int kraLibraryId);

        public Task<ResponseModel> CreateorUpdateKRALibrary(KRALibrary kraLibraryModel);

        public Task<ResponseModel> DeleteKRALibrary(int kraLibraryId);

        public Task<KRAWeightage> GetKRAWeightageDetailsByKRALibraryId(int kraLibraryId);

        public Task<List<KRALibrary>> GetKRADetailsByWeightageId(int weightageId);
    }
}

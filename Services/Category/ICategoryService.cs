using DF_PA_API.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using DF_EvolutionAPI.ViewModels;

namespace DF_PA_API.Services
{
    public interface ICategoryService
    {
        public Task<ResponseModel> CreateCategory(Category categoryModel);
        public Task<ResponseModel> UpdateCategory(Category categoryModel);
        public Task<List<Category>> GetAllCategories();
        public Task<ResponseModel> DeleteCategoryById(int id);
    }
}

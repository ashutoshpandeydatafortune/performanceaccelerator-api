using System;
using System.Linq;
using DF_EvolutionAPI;
using DF_PA_API.Models;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using DF_EvolutionAPI.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DF_PA_API.Services.DesignatedRoles;

namespace DF_PA_API.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly DFEvolutionDBContext _dbContext;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(DFEvolutionDBContext dbContext, ILogger<CategoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseModel> CreateCategory(Category categoryModel)
        {
            ResponseModel model = new ResponseModel();

            try
            {
                var category = await _dbContext.Categories.FirstOrDefaultAsync(category => category.CategoryName == categoryModel.CategoryName && category.IsActive == (int)Status.IS_ACTIVE);
                if (category == null)
                {
                    categoryModel.IsActive = (int)Status.IS_ACTIVE;
                    categoryModel.CreateDate = DateTime.UtcNow;

                    await _dbContext.AddAsync(categoryModel); // Async addition
                    model.Messsage = "Category saved successfully.";

                    await _dbContext.SaveChangesAsync(); // Ensure this is also async
                    model.IsSuccess = true;
                }
                else
                {
                    model.Messsage = "Category already exists.";
                    model.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }
            return model;
        }

        public async Task<ResponseModel> UpdateCategory(Category categoryModel)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var category = _dbContext.Categories.FirstOrDefault(category => category.CategoryName == categoryModel.CategoryName && category.Description == categoryModel.Description && category.IsActive == (int)Status.IS_ACTIVE);
                if (category != null)
                {
                    model.IsSuccess = false;
                    model.Messsage = "Skill with the same name already exists.";
                    return model;
                }
                else
                {

                    Category updateCategory = _dbContext.Categories.Find(categoryModel.CategoryId);
                    if (updateCategory != null)
                    {
                        updateCategory.CategoryName = categoryModel.CategoryName;
                        updateCategory.Description = categoryModel.Description;
                        updateCategory.IsActive = (int)Status.IS_ACTIVE;
                        updateCategory.UpdateBy = categoryModel.UpdateBy;
                        updateCategory.UpdateDate = DateTime.Now;

                        await _dbContext.SaveChangesAsync();

                        model.Messsage = "Category updated successfully.";
                        model.IsSuccess = true;
                    }

                    else
                    {
                        model.Messsage = "Category already exist.";
                        model.IsSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
            }
            return model;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _dbContext.Categories.Where(Category => Category.IsActive == (int)Status.IS_ACTIVE).OrderBy(category => category.CategoryName).ToListAsync();
        }

        public async Task<ResponseModel> DeleteCategoryById(int id)
        {
            ResponseModel model = new ResponseModel();
            try
            {
                var deleteCategory = _dbContext.Categories.Find(id);
                if (deleteCategory != null)
                {
                    deleteCategory.IsActive = (int)Status.IN_ACTIVE;
                    deleteCategory.UpdateDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    model.IsSuccess = true;
                    model.Messsage = "Category deleted successfully.";
                }
                else
                {
                    model.IsSuccess = false;
                    model.Messsage = "Category not found.";
                }
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

            }
            return model;
        }
    }
}

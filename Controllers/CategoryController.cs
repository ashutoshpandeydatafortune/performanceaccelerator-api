using System;
using DF_PA_API.Models;
using DF_PA_API.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DF_PA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private ICategoryService _categoryService;

        public CategoryController(ICategoryService skillService)
        {
            _categoryService = skillService;
        }

        /// <summary>
        /// It is used to insert the category.
        /// </summary>
        /// <param name="categoryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateCategory(Category categoryModel)
        {
            try
            {
                var response = await _categoryService.CreateCategory(categoryModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to update the category.
        /// </summary>
        /// <param name="categoryModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateCategory(Category categoryModel)
        {
            try
            {
                var response = await _categoryService.UpdateCategory(categoryModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to get all the category.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var response = await _categoryService.GetAllCategories();
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

         /// <summary>
        /// It is used to delete the category.
        /// </summary>
        /// <param> id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteCategoryById/{id}")]
        public async Task<IActionResult> DeleteCategoryById(int id)
        {
            try
            {
                var response = await _categoryService.DeleteCategoryById(id);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

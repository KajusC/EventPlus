using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryLogic _categoryLogic;
        public CategoryController(ICategoryLogic categoryLogic)
        {
            _categoryLogic = categoryLogic;
        }
        [HttpGet]
        public async Task<ActionResult<List<CategoryViewModel>>> GetAllCategories()
        {
            var categories = await _categoryLogic.GetAllCategoriesAsync();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryViewModel>> GetCategoryById(int id)
        {
            var category = await _categoryLogic.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        [HttpPost]
        public async Task<ActionResult<bool>> CreateCategory([FromBody] CategoryViewModel category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null");
            }
            var result = await _categoryLogic.CreateCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.IdCategory }, result);
        }
        [HttpPut]
        public async Task<ActionResult<bool>> UpdateCategory([FromBody] CategoryViewModel category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null");
            }
            var result = await _categoryLogic.UpdateCategoryAsync(category);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be greater than zero.");
            }
            var result = await _categoryLogic.DeleteCategoryAsync(id);
            return Ok(result);
        }
    }
}

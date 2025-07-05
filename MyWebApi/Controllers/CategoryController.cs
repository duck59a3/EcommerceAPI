using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Data;
using MyWebApi.DTOs.Conversions;
using MyWebApi.Models;
using MyWebApi.Responses;
using MyWebApi.Services;
using MyWebApi.Services.IService;
using System.Threading.Tasks;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategory()
        {
            var categories = await _categoryService.GetAllAsync();
            if (!categories.Any())
            {
                return NotFound("Không tìm thấy loại hàng.");
            }
            return Ok(categories);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _categoryService.FindByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _categoryService.CreateAsync(category);
            return response.Flag is true ? Ok(response) : BadRequest(response);

        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
         
            var response = await _categoryService.UpdateAsync(category);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        [HttpDelete]
        public async Task<ActionResult<Response>> Delete(int id)
        {
            var category = await _categoryService.FindByIdAsync(id);
            var response = await _categoryService.DeleteAsync(category);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}

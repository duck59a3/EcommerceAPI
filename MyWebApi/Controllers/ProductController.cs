using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs;
using MyWebApi.DTOs.Conversions;
using MyWebApi.DTOs.Requests;
using MyWebApi.Helpers;
using MyWebApi.Helpers.QueryParameters;
using MyWebApi.Models;
using MyWebApi.Responses;
using MyWebApi.Services;
using MyWebApi.Services.IService;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ProductDTO>>> GetAllProducts([FromQuery] ProductQuery query)
        {
            // Lấy tất cả sản phẩm từ repo
            var products = await _productService.GetProductsAsync(query);
            if (!products.Data.Any())
            {
                return NotFound("No products found.");
            }
            // Chuyển đổi danh sách sản phẩm sang DTO
            var (_,list) = ProductConversion.FromEntity(null!,products.Data);
            return list!.Any() ? Ok(list) : NotFound("No products found.");
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProduct(int id)
        {
            // Lấy sản phẩm theo ID từ repo
            var product = await _productService.FindByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            // Chuyển đổi sản phẩm sang DTO
            var (single, _) = ProductConversion.FromEntity(product, null!);
            return single != null ? Ok(single) : NotFound($"Product with ID {id} not found.");
        }
        [HttpPost]
        [Authorize(Roles = UserRole.Admin)] // Chỉ cho phép người dùng có vai trò Admin 
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
           if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entityexist = await _productService.FindByIdAsync(product.Id);
            if (entityexist is not null)
            {
                return new Response(false, "Sản phẩm đã tồn tại");
            }
            //chuyển sang entity
            var productEntity = ProductConversion.ToEntity(product);
            var response = await _productService.CreateAsync(productEntity);
            return response.Flag is true? Ok(response) : BadRequest(response);

        }

        [HttpPut]
        [Authorize(Roles = UserRole.Admin)] // Chỉ cho phép người dùng có vai trò Admin
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //chuyển sang entity
            var productEntity = ProductConversion.ToEntity(product);
            var response = await _productService.UpdateAsync(productEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        [HttpDelete]
        [Authorize(Roles = UserRole.Admin)] // Chỉ cho phép người dùng có vai trò Admin
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            //chuyển sang entity
            var productEntity = ProductConversion.ToEntity(product);
            var response = await _productService.DeleteAsync(productEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        [HttpPost("upload-image")]
        public async Task<ActionResult<Response>> UploadImage([FromForm] UploadProductImageDTO model)
        {
            if (model.image == null || model.image.Length == 0)
                return BadRequest("Không có file nào được tải lên");
            var response = await _productService.UploadProductImage(model.productId ,model.image);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        [HttpPost("upload-video")]
        public async Task<ActionResult<Response>> UploadVideo([FromForm] UploadProductImageDTO model)
        {
            if (model.image == null || model.image.Length == 0)
                return BadRequest("Không có file nào được tải lên");
            var response = await _productService.UploadProductVideo(model.productId, model.image);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}

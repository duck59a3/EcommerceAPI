using Castle.Core.Logging;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Controllers;
using MyWebApi.DTOs;
using MyWebApi.Helpers.QueryParameters;
using MyWebApi.Models;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace UnitTests.Controllers
{
    public class ProductControllerTests
    {
        private readonly IProductService _productService;
        private readonly ProductController _productController;
        private readonly ILogger<ProductController> _logger;

        public ProductControllerTests()
        {
            _productService = A.Fake<IProductService>();
            _logger = A.Fake<ILogger<ProductController>>();
            _productController = new ProductController(_productService, _logger); // Pass logger to constructor  
        }
        [Fact]
        // GetAll Products  
        public async Task GetProducts_ShouldReturnOkProduct_WhenExists()
        {
            // Arrange  
            var query = new ProductQuery();
            var productlst = new List<Product>()
            {
                new() { Id = 1, Name = "Shirt", Description = "Aji ngon", Price = 10000, Quantity = 10, Size = "M", Color = "Red", Material = "Da"},
                new() {Id = 2, Name = "Dress", Description = "Aji fffngon", Price = 20000, Quantity = 20, Size = "M", Color = "White", Material = "Da"}
            };
            // Fake response  
            var pagedResponse = new PagedResponse<Product>(productlst, 1, 10, 2);
            A.CallTo(() => _productService.GetProductsAsync(query)).Returns(Task.FromResult(pagedResponse));
            //Act
            var result = await _productController.GetAllProducts(query);
            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull(); 
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedProducts = okResult.Value as List<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts.First().Id.Should().Be(1);
            returnedProducts.Last().Id.Should().Be(2);
        }
        [Fact]
        public async Task GetProducts_ReturnNotFound_WhenNotExists()
        {
            var query = new ProductQuery();
            var productlst = new List<Product>();
            var pagedResponse = new PagedResponse<Product>(productlst, 1, 10, 2);
            A.CallTo(() => _productService.GetProductsAsync(query)).Returns(Task.FromResult(pagedResponse));
            var result = await _productController.GetAllProducts(query);
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var message = notFoundResult.Value as string;
            message.Should().Be("No products found.");

        }
    }
}

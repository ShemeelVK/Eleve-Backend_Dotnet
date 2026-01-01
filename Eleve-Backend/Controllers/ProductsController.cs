using AutoMapper;
using Eleve_Backend.Application.DTOs.Products;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eleve_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService,IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }


        //[Authorize(Roles ="User")]
        [HttpGet("Get-All-Product")]
        public IActionResult GetAll()
        {
            return Ok(_productService.GetAllProducts());
        }

        [HttpGet("Featured-Products")]
        public IActionResult GetFeaturedProducts()
        {
            var products = _productService.GetFeaturedProduct();
            return Ok(products);
        }

        [HttpGet("Product-By-Id")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound("Product not found");
            return Ok(product);
        }

        [HttpGet("Product-By-Category")]
        public IActionResult GetByCategory(string category)
        {
            var products=_productService.GetProductsByCategory(category);
            return Ok(products);
        }

        [Authorize]
        [HttpPost("Add-Product")]
        public IActionResult Create([FromBody] CreateProductDto request)
        {
            try
            {
                var productEntity = _mapper.Map<Product>(request);

                var createdProduct = _productService.AddProduct(productEntity);
                return Ok(createdProduct);
            }
            catch(InvalidOperationException ex)
            {
                //logic for already exists
                return Conflict(new { message = ex.Message   });
            }
            catch(Exception ex)
            {
                return BadRequest(new {error= ex.Message });
            }
            //product.Id = 0; //for generating a new id for sql with the identity



        }

        [Authorize]
        [HttpPut("Update-Product")]
        public IActionResult Update(int id, [FromForm] CreateProductDto request)
        {
            //using automapper to convert DTO -> Entity
            var productEntity = _mapper.Map<Product>(request);

            //ensuring the id is correct before saving
            productEntity.Id = id;

            //service
            var existing = _productService.GetProductById(id);
            if (existing == null)
                return NotFound("Product not found");

            _productService.UpdateProduct(id, productEntity);

            return Ok("Product updated successfully");
        }

        [Authorize]
        [HttpDelete("Delete-Product")]
        public IActionResult Delete(int id)
        {
            var existing=_productService.GetProductById(id);

            if (existing == null)
                return NotFound("Product Not found");

            _productService.DeleteProduct(id);
            return Ok("Product Deleted");
        }
    }
}

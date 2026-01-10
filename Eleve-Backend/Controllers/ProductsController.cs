using AutoMapper;
using Eleve_Backend.Application.DTOs.Products;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eleve_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly EleveDbContext _context;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService,IMapper mapper, EleveDbContext context)
        {
            _productService = productService;
            _mapper = mapper;
            _context = context;
        }


        //[Authorize(Roles ="User")]
        [HttpGet("Get-All-Product")]
        public IActionResult GetAll([FromQuery] string? sortOrder = null)
        {
            return Ok(_productService.GetAllProducts(sortOrder));
        }

        [HttpGet("Featured-Products")]
        public IActionResult GetFeaturedProducts()
        {
            var products = _productService.GetFeaturedProduct();
            return Ok(products);
        }

        [Authorize]
        [HttpGet("Admin-Products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsAdmin()
        {
            var products = await _context.Products
         .Select(p => new ProductDto
         {
             Id = p.Id,
             Name = p.Name,
             Price = p.Price,
             Description = p.Description,
             Category = p.Category ?? "Uncategorized",
             Stock = p.Stock,
             ImageUrl = p.ImageUrl,
             IsFeatured = p.IsFeatured,
             IsDeleted = p.IsDeleted
         })
         .ToListAsync();

         
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
        public IActionResult GetByCategory([FromQuery] string category, [FromQuery] string? sortOrder = null)
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
        [HttpPut("Update-Product/{id}")]
        public IActionResult Update(int id, [FromBody] CreateProductDto request)
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

        [Authorize(Roles ="Admin")]
        [HttpDelete("Delete-Product/{id}")]
        public IActionResult Delete(int id)
        {
            var existing=_productService.GetProductById(id);

            if (existing == null)
                return NotFound("Product Not found");

            _productService.DeleteProduct(id);
            return Ok("Product Deleted");
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var products = await _productService.SearchProductsAsync(query);
            return Ok(products);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("Admin-Search")]
        public async Task<IActionResult> AdminSearch([FromQuery] string query, [FromQuery] string? sortOrder = null)
        {
            var products = await _productService.SearchProductsAsync(query,sortOrder);
            return Ok(products);
        }


    }
}

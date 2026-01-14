using AutoMapper;
using Eleve_Backend.Application.DTOs.Products;
using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Infrastructure.Persistence;
using Eleve_Backend.Infrastructure.Services;
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
        private readonly IPhotoService _photoService;

        public ProductsController(IProductService productService,IMapper mapper, EleveDbContext context, IPhotoService photoService)
        {
            _productService = productService;
            _mapper = mapper;
            _context = context;
            _photoService = photoService;
        }


        //[Authorize(Roles ="Customer")]
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

        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Create([FromForm] CreateProductDto request)
        {
            try
            {
                
                if (request.ImageFile == null || request.ImageFile.Length == 0)
                {
                    return BadRequest(new { message = "Product image is required." });
                }

                var photoResult = await _photoService.AddPhotoAsync(request.ImageFile);

                if (photoResult.Error != null)
                {
                    return BadRequest(new { message = photoResult.Error.Message });
                }

                // 2. Map DTO to Entity
                var productEntity = _mapper.Map<Product>(request);

                // 3. Inject the Cloudinary URL into the entity manually
                productEntity.ImageUrl = photoResult.SecureUrl.AbsoluteUri;

                // 4. Call your existing Service (No changes needed in Service!)
                var createdProduct = _productService.AddProduct(productEntity);

                return Ok(createdProduct);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            

        }

        [Authorize]
        [HttpPut("Update-Product/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CreateProductDto request)
        {
          
            try
            {
                
                var existingProduct = _productService.GetProductById(id);
                if (existingProduct == null) return NotFound("Product not found");

             
                if (request.ImageFile != null && request.ImageFile.Length > 0)
                {
                    var photoResult = await _photoService.AddPhotoAsync(request.ImageFile);
                    if (photoResult.Error != null) return BadRequest(new { message = photoResult.Error.Message });

                    
                    existingProduct.ImageUrl = photoResult.SecureUrl.AbsoluteUri;
                }

     
                existingProduct.Name = request.Name;
                existingProduct.Description = request.Description;
                existingProduct.Price = request.Price;
                existingProduct.Category = request.Category;
                existingProduct.Stock = request.Stock;
                existingProduct.IsFeatured = request.IsFeatured;

                
                _productService.UpdateProduct(id, existingProduct);

                return Ok(new { message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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

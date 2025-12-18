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

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        //[Authorize(Roles ="User")]
        [HttpGet("Get All Product")]
        public IActionResult GetAll()
        {
            return Ok(_productService.GetAllProducts());
        }


        [HttpGet("Get Product By Id {id}")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound("Product not found");
            return Ok(product);
        }

        [Authorize]
        [HttpPost("Add Product")]
        public IActionResult Create([FromBody] Product product)
        {
            product.Id = 0; //for generating a new id for sql with the identity

            var createdProduct = _productService.AddProduct(product);

            return Ok(createdProduct);
        }
    }
}

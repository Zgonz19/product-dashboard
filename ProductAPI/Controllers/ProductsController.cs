using Microsoft.AspNetCore.Mvc;
using ProductAPI.DTOs;
using ProductAPI.Middleware;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> AllActiveProducts()
        {
            var products = await _productService.GetActiveProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                var created = await _productService.CreateProduct(dto);
                return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
        {
            try
            {
                var updated = await _productService.UpdateProduct(id, dto);
                if (updated is null)
                    return NotFound();

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductById(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<ProductDto>>> SearchProducts([FromQuery] ProductSearchQuery query)
        {
            var result = await _productService.SearchProducts(query);
            return Ok(result);
        }
    }
}

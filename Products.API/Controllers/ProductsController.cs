using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Models;
using Products.API.Services;
using System;
using System.Threading.Tasks;

namespace Products.API.Controllers
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request) // 1. Se cambió 'r' por 'request' para que coincida abajo.
        { 
            // 1. Mapeo del DTO al Modelo
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = (double)request.Price,
                Stock = request.Stock,
                Category = request.Category,
                CreatedAt = DateTime.UtcNow.ToString("O")
            };

            var createdProduct = await _productService.CreateProductAsync(product);

            return CreatedAtAction(nameof(Create), new { id = createdProduct.Id }, createdProduct);
        } 
    }
}
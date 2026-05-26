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

        // Inyección de dependencias (Faltaba definir el constructor correctamente)
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request) // 1. Se cambió 'r' por 'request' para que coincida abajo. 2. Faltaba cerrar el paréntesis ')'.
        { // 3. Faltaba la llave de apertura del método.
            // 1. Mapeo del DTO al Modelo
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Stock = request.Stock,
                Categoria = request.Categoria,
                FechaCreacion = DateTime.UtcNow
            };

            // 2. Ejecutar lógica de negocio (Se descomentó y se cerró el paréntesis correctamente)
            var createdProduct = await _productService.CreateProductAsync(product);

            // 3. Devolver 201 Created según el PDF (Se cerró el paréntesis correctamente)
            return CreatedAtAction(nameof(Create), new { id = createdProduct.Id }, createdProduct);
        } // 4. Faltaba la llave de cierre del método.
    }
}
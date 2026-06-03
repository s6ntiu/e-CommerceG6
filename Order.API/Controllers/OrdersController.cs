using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Order.API.Data;
using Order.API.Models;
using ECommerce.Shared.Exceptions;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderRepository _repo;

        public OrdersController(OrderRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(string id)
        {
            var order = await _repo.GetOrderAsync(id);
            if (order == null) throw new NotFoundException("ORD-001", "Orden no encontrada.");
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new BusinessRuleException("ORD-002", "Los datos de la orden son inválidos.");

            // TODO: Integración HTTP con Users API (ORD-003) y Products API (ORD-004, ORD-005)
            // throw new NotFoundException("ORD-003", "Usuario no encontrado al crear la orden.");
            // throw new UnprocessableEntityException("ORD-005", "Stock insuficiente.");

            // Simulación de armado de la orden (Asumimos Precio = 1500 provisorio)
            var newOrder = new OrderDTO
            {
                Id = Guid.NewGuid().ToString(),
                UsuarioId = request.UsuarioId,
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow,
                Items = request.Items.Select(i => new OrderItemDTO { 
                    ProductoId = i.ProductoId, 
                    Cantidad = i.Cantidad, 
                    PrecioUnitario = 1500m // Esto debería venir de la API de Products
                }).ToList()
            };
            newOrder.Total = newOrder.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

            await _repo.CreateOrderAsync(newOrder);

            return Created($"/api/orders/{newOrder.Id}", newOrder);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateOrderStatusRequest request)
        {
            var order = await _repo.GetOrderAsync(id);
            if (order == null) throw new NotFoundException("ORD-001", "Orden no encontrada.");

            // Regla de negocio simple
            if (order.Estado == "Entregada" && request.Estado == "Pendiente")
                throw new BusinessRuleException("ORD-006", "Una orden en estado 'Entregada' no puede volver a 'Pendiente'.");

            await _repo.UpdateStatusAsync(id, request.Estado);

            return Ok(new { id = id, estado = request.Estado, fechaActualizacion = DateTime.UtcNow });
        }
    }
}

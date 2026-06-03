using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Order.API.Data;
using Order.API.Models;
using ECommerce.Shared.Exceptions;
using System.Net.Http;
using System.Net.Http.Json;
using Order.API.DTOs;
namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderRepository _repo;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersController(OrderRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _httpClientFactory = httpClientFactory;
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

            var usersClient = _httpClientFactory.CreateClient("UsersAPI");
            var userResponse = await usersClient.GetAsync($"/api/users/{request.UsuarioId}");
            if (!userResponse.IsSuccessStatusCode)
                throw new NotFoundException("ORD-003", "Usuario no encontrado al crear la orden.");

            var productsClient = _httpClientFactory.CreateClient("ProductsAPI");
            var orderItems = new System.Collections.Generic.List<OrderItemDTO>();

            foreach (var item in request.Items)
            {
                var productResponse = await productsClient.GetAsync($"/api/products/{item.ProductoId}");
                if (!productResponse.IsSuccessStatusCode)
                    throw new NotFoundException("ORD-004", $"Producto {item.ProductoId} no encontrado al crear la orden.");

                var product = await productResponse.Content.ReadFromJsonAsync<ProductDTO>();
                if (product == null)
                    throw new NotFoundException("ORD-004", $"Producto {item.ProductoId} no encontrado al crear la orden.");

                if (product.Stock < item.Cantidad)
                    throw new UnprocessableEntityException("ORD-005", $"Stock insuficiente para '{product.Nombre}'. Disponible: {product.Stock}, solicitado: {item.Cantidad}.");

                orderItems.Add(new OrderItemDTO {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = product.Precio
                });
            }

            var newOrder = new OrderDTO
            {
                Id = Guid.NewGuid().ToString(),
                UsuarioId = request.UsuarioId,
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow,
                Items = orderItems
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

            if (request.Estado == "Confirmada")
            {
                // Vaciar el carrito
                var cartClient = _httpClientFactory.CreateClient("CartAPI");
                await cartClient.DeleteAsync($"/api/carts/{order.UsuarioId}");

                // Enviar notificación
                var notifClient = _httpClientFactory.CreateClient("NotificationsAPI");
                await notifClient.PostAsJsonAsync("/api/notifications/send", new 
                {
                    UsuarioId = order.UsuarioId,
                    Mensaje = $"Su orden #{id} fue confirmada.",
                    Tipo = "Email"
                });
            }
            return Ok(new { id = id, estado = request.Estado, fechaActualizacion = DateTime.UtcNow });
        }
    }
}

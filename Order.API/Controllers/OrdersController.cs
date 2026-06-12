using Microsoft.AspNetCore.Mvc;
using Order.API.Data;
using Order.API.DTOs;
using ECommerce.Shared.Exceptions;
using System;
using System.Threading.Tasks;

using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderRepository _repository;
    public OrdersController(OrderRepository repository) {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? usuarioId)
    {
        var orders = await _repository.GetAllAsync(usuarioId);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null) throw new NotFoundException("ORD-001", "Orden no encontrada.");
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, [FromServices] IHttpClientFactory httpClientFactory)
    {
        bool hasItems = false;
        if (request.Items != null) {
            foreach (var item in request.Items) {
                hasItems = true;
                break;
            }
        }
        if (!hasItems)
            throw new BusinessRuleException("ORD-002", "Los datos de la orden son inválidos.");

        // Validar Usuario (ORD-003) en Users.API
        var usersClient = httpClientFactory.CreateClient("UsersAPI");
        var userRes = await usersClient.GetAsync($"/api/users/{request.UsuarioId}"); 
        if (!userRes.IsSuccessStatusCode) 
            throw new NotFoundException("ORD-003", "Usuario no encontrado al crear la orden.");

        var productsClient = httpClientFactory.CreateClient("ProductsAPI");
        var newItems = new List<Models.OrderItem>();
        decimal total = 0;

        foreach (var item in request.Items!)
        {
            var prodRes = await productsClient.GetAsync($"/api/products/{item.ProductoId}");
            if (!prodRes.IsSuccessStatusCode) 
                throw new NotFoundException("ORD-004", "Producto no encontrado al crear la orden.");
            
            var prodJson = await prodRes.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(prodJson);
            var stock = doc.RootElement.GetProperty("stock").GetInt32();
            var precio = doc.RootElement.GetProperty("precio").GetDecimal();
            var nombre = doc.RootElement.GetProperty("nombre").GetString();

            if (stock < item.Cantidad)
                throw new BusinessRuleException("ORD-005", $"Stock insuficiente para '{nombre}'. Disponible: {stock}, solicitado: {item.Cantidad}."); 

            newItems.Add(new Models.OrderItem { ProductoId = item.ProductoId, Cantidad = item.Cantidad, PrecioUnitario = precio });
            total += precio * item.Cantidad;
        }

        var newOrder = new Models.Order { UsuarioId = request.UsuarioId, Items = newItems, Total = total };
        await _repository.CreateAsync(newOrder);
        
        return CreatedAtAction(nameof(GetById), new { id = newOrder.Id }, newOrder);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null) throw new NotFoundException("ORD-001", "Orden no encontrada.");
        
        if (order.Estado == "Entregada" || order.Estado == "Cancelada")
            throw new BusinessRuleException("ORD-006", "El estado de la orden no puede ser modificado.");

        await _repository.UpdateStatusAsync(id, request.Estado);
        order.Estado = request.Estado;
        return Ok(order);
    }
}

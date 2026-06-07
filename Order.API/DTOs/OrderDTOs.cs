using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Order.API.DTOs;

public class CreateOrderRequest
{
    [Required] public Guid UsuarioId { get; set; }
    [Required, MinLength(1)] public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    [Required] public int ProductoId { get; set; }
    [Required, Range(1, int.MaxValue)] public int Cantidad { get; set; }
}

public class UpdateOrderStatusRequest
{
    [Required] public string Estado { get; set; } = string.Empty;
}

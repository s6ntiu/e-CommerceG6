using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Order.API.Models
{
    public class OrderDTO
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    public class OrderItemDTO
    {
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class CreateOrderRequest
    {
        [Required] public string UsuarioId { get; set; } = string.Empty;
        [Required, MinLength(1, ErrorMessage = "ORD-002: Items vacíos.")]
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        [Required] public string ProductoId { get; set; } = string.Empty;
        [Required, Range(1, int.MaxValue)] public int Cantidad { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        [Required] public string Estado { get; set; } = string.Empty;
    }
}

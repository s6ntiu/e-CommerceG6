using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Order.API.Models;

namespace Order.API.Data
{
    public class OrderRepository
    {
        private readonly IConfiguration _config;
        public OrderRepository(IConfiguration config) => _config = config;
        private SqliteConnection CreateConnection() => new(_config.GetConnectionString("DefaultConnection") ?? "Data Source=order.db");

        public async Task<OrderDTO?> GetOrderAsync(string orderId)
        {
            using var conn = CreateConnection();
            var order = await conn.QuerySingleOrDefaultAsync<OrderDTO>(
                "SELECT id, usuario_id as UsuarioId, total, estado, fecha_creacion as FechaCreacion FROM orders WHERE id = @Id", 
                new { Id = orderId });

            if (order == null) return null;

            var items = await conn.QueryAsync<OrderItemDTO>(
                "SELECT producto_id as ProductoId, cantidad, precio_unitario as PrecioUnitario FROM order_items WHERE order_id = @OrderId", 
                new { OrderId = orderId });

            order.Items = items.ToList();
            return order;
        }

        public async Task CreateOrderAsync(OrderDTO order)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "INSERT INTO orders (id, usuario_id, total, estado, fecha_creacion) VALUES (@Id, @UsuarioId, @Total, @Estado, @FechaCreacion)", 
                new { order.Id, order.UsuarioId, order.Total, order.Estado, FechaCreacion = order.FechaCreacion.ToString("O") });

            foreach(var item in order.Items)
            {
                await conn.ExecuteAsync(
                    "INSERT INTO order_items (order_id, producto_id, cantidad, precio_unitario) VALUES (@OrderId, @ProductoId, @Cantidad, @PrecioUnitario)", 
                    new { OrderId = order.Id, item.ProductoId, item.Cantidad, item.PrecioUnitario });
            }
        }

        public async Task UpdateStatusAsync(string orderId, string nuevoEstado)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "UPDATE orders SET estado = @Estado, fecha_actualizacion = @Fecha WHERE id = @Id", 
                new { Estado = nuevoEstado, Fecha = DateTime.UtcNow.ToString("O"), Id = orderId });
        }
    }
}

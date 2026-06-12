using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Order.API.Models;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Order.API.Data;

public class OrderRepository
{
    private readonly IConfiguration _config;
    public OrderRepository(IConfiguration config) {
        _config = config;
    }
    private SqliteConnection CreateConnection() {
        return new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=order.db");
    }

    public async Task<IEnumerable<Models.Order>> GetAllAsync(Guid? usuarioId)
    {
        using var conn = CreateConnection();
        var sql = "SELECT id as Id, usuario_id as UsuarioId, total as Total, estado as Estado, fecha_creacion as FechaCreacion FROM orders";
        if (usuarioId.HasValue) sql += " WHERE usuario_id = @UsuarioId";
        sql += " ORDER BY fecha_creacion DESC";
        
        return await conn.QueryAsync<Models.Order>(sql, new { UsuarioId = usuarioId?.ToString() });
    }

    public async Task<Models.Order?> GetByIdAsync(Guid id)
    {
        using var conn = CreateConnection();
        var order = await conn.QuerySingleOrDefaultAsync<Models.Order>(
            "SELECT id as Id, usuario_id as UsuarioId, total as Total, estado as Estado, fecha_creacion as FechaCreacion FROM orders WHERE id = @Id", 
            new { Id = id.ToString() });

        if (order != null)
        {
            var items = await conn.QueryAsync<OrderItem>(
                "SELECT producto_id as ProductoId, cantidad as Cantidad, precio_unitario as PrecioUnitario FROM order_items WHERE order_id = @OrderId", 
                new { OrderId = id.ToString() });
            order.Items = items.ToList();
        }
        return order;
    }

    public async Task<Models.Order> CreateAsync(Models.Order order)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var transaction = conn.BeginTransaction();
        try
        {
            await conn.ExecuteAsync(
                "INSERT INTO orders (id, usuario_id, total, estado, fecha_creacion) VALUES (@Id, @UsuarioId, @Total, @Estado, @FechaCreacion)",
                new { Id = order.Id.ToString(), UsuarioId = order.UsuarioId.ToString(), order.Total, order.Estado, FechaCreacion = order.FechaCreacion.ToString("O") }, transaction);

            foreach (var item in order.Items)
            {
                await conn.ExecuteAsync(
                    "INSERT INTO order_items (order_id, producto_id, cantidad, precio_unitario) VALUES (@OrderId, @ProductoId, @Cantidad, @PrecioUnitario)",
                    new { OrderId = order.Id.ToString(), ProductoId = item.ProductoId.ToString(), item.Cantidad, item.PrecioUnitario }, transaction);
            }
            transaction.Commit();
            return order;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateStatusAsync(Guid id, string nuevoEstado)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("UPDATE orders SET estado = @Estado WHERE id = @Id", new { Estado = nuevoEstado, Id = id.ToString() });
    }
}

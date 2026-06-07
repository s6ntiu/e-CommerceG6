using Cart.API.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace Cart.API.Data;

public class CartRepository
{
    private readonly string _connectionString;

    public CartRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db";
    }

    private SqliteConnection CreateConnection() => new SqliteConnection(_connectionString);

    public async Task<Models.Cart?> GetActiveCartByUserIdAsync(string userId)
    {
        using var conn = CreateConnection();
        var cart = await conn.QuerySingleOrDefaultAsync<Models.Cart>(
            "SELECT id as Id, user_id as UserId, status as Status, created_at as CreatedAt, updated_at as UpdatedAt FROM carts WHERE user_id = @UserId AND status = 'Active'",
            new { UserId = userId });

        if (cart != null)
        {
            var items = await conn.QueryAsync<CartItem>(
                "SELECT id as Id, cart_id as CartId, product_id as ProductId, quantity as Quantity, unit_price as UnitPrice FROM cart_items WHERE cart_id = @CartId",
                new { CartId = cart.Id });
            cart.Items = items.ToList();
        }

        return cart;
    }

    public async Task<int> CreateCartAsync(string userId)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "INSERT INTO carts (user_id) VALUES (@UserId); SELECT last_insert_rowid();",
            new { UserId = userId });
    }

    public async Task AddOrUpdateItemAsync(int cartId, int productId, int quantity, decimal unitPrice)
    {
        using var conn = CreateConnection();
        var existingItem = await conn.QuerySingleOrDefaultAsync<CartItem>(
            "SELECT * FROM cart_items WHERE cart_id = @CartId AND product_id = @ProductId",
            new { CartId = cartId, ProductId = productId });

        if (existingItem != null)
        {
            await conn.ExecuteAsync(
                "UPDATE cart_items SET quantity = quantity + @Quantity WHERE id = @Id",
                new { Quantity = quantity, Id = existingItem.Id });
        }
        else
        {
            await conn.ExecuteAsync(
                "INSERT INTO cart_items (cart_id, product_id, quantity, unit_price) VALUES (@CartId, @ProductId, @Quantity, @UnitPrice)",
                new { CartId = cartId, ProductId = productId, Quantity = quantity, UnitPrice = unitPrice });
        }

        // Actualizar el timestamp del carrito
        await conn.ExecuteAsync("UPDATE carts SET updated_at = datetime('now', 'localtime') WHERE id = @CartId", new { CartId = cartId });
    }

    public async Task RemoveItemAsync(int cartId, int productId)
    {
        using var conn = CreateConnection();
        var affected = await conn.ExecuteAsync(
            "DELETE FROM cart_items WHERE cart_id = @CartId AND product_id = @ProductId",
            new { CartId = cartId, ProductId = productId });

        if (affected > 0)
        {
            await conn.ExecuteAsync("UPDATE carts SET updated_at = datetime('now', 'localtime') WHERE id = @CartId", new { CartId = cartId });
        }
    }
}

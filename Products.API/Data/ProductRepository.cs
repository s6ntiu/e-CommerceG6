using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Products.API.DTOs;
using Products.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Products.API.Data;
public class ProductRepository
{
private readonly IConfiguration _config;
public ProductRepository(IConfiguration config) {
    _config = config;
}
private SqliteConnection CreateConnection() {
    return new SqliteConnection(_config.GetConnectionString("DefaultConnection") ?? "Data Source=products.db");
}
public async Task<IEnumerable<Product>> GetAllAsync()
{
using var conn = CreateConnection();
return await conn.QueryAsync<Product>("SELECT id, name, description, price, stock, created_at AS CreatedAt, updated_at AS UpdatedAt FROM products ORDER BY id DESC");
}
public async Task<Product?> GetByIdAsync(int id)
{
using var conn = CreateConnection();
return await conn.QuerySingleOrDefaultAsync<Product>("SELECT id, name, description, price, stock, created_at AS CreatedAt, updated_at AS UpdatedAt FROM products WHERE id = @id", new { id });
}
public async Task<Product> CreateAsync(CreateProductRequest request)
{
using var conn = CreateConnection();
var exists = await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM products WHERE name = @Name", new { request.Name });
if (exists > 0) {
    throw new ECommerce.Shared.Exceptions.BusinessRuleException("PRD-003", $"Ya existe un producto con ese nombre en la base de datos.");
}
var id = await conn.ExecuteScalarAsync<int>("INSERT INTO products (name, description, price, stock) VALUES (@Name, @Description, @Price, @Stock); SELECT last_insert_rowid();", request);
return (await GetByIdAsync(id))!;
}
public async Task<bool> UpdateAsync(int id, UpdateProductRequest request)
{
using var conn = CreateConnection();
var rows = await conn.ExecuteAsync("UPDATE products SET name = @Name, description = @Description, price = @Price, stock = @Stock, updated_at = datetime('now') WHERE id = @Id", new { request.Name, request.Description, request.Price, request.Stock, Id = id });
return rows > 0;
}
public async Task<bool> DeleteAsync(int id)
{
using var conn = CreateConnection();
var rows = await conn.ExecuteAsync("DELETE FROM products WHERE id = @id", new { id });
return rows > 0;
}
}

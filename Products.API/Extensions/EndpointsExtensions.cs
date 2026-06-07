using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Products.API.Data;
using Products.API.DTOs;
namespace Products.API.Extensions;
public static class EndpointsExtensions
{
public static void MapProductEndpoints(this WebApplication app)
{
app.MapGet("/api/products", async (ProductRepository repo) => { var products = await repo.GetAllAsync(); return Results.Ok(products); }).WithTags("Products");
app.MapGet("/api/products/{id}", async (int id, ProductRepository repo) => { var product = await repo.GetByIdAsync(id); return product is not null ? Results.Ok(product) : Results.NotFound(); }).WithTags("Products");
app.MapPost("/api/products", async (CreateProductRequest req, ProductRepository repo) => { var product = await repo.CreateAsync(req); return Results.Ok(product); }).WithTags("Products");
app.MapPut("/api/products/{id}", async (int id, UpdateProductRequest req, ProductRepository repo) => { var updated = await repo.UpdateAsync(id, req); return updated ? Results.Ok(req) : Results.NotFound(); }).WithTags("Products");
app.MapDelete("/api/products/{id}", async (int id, ProductRepository repo) => { var deleted = await repo.DeleteAsync(id); return deleted ? Results.Ok() : Results.NotFound(); }).WithTags("Products");
}
}

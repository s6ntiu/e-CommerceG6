using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Products.API.Data;
using Products.API.DTOs;
using System.Threading.Tasks;

namespace Products.API.Extensions;

public static class EndpointsExtensions
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        // GET ALL
        async Task<IResult> GetAllProducts(ProductRepository repo)
        {
            var products = await repo.GetAllAsync();
            return Results.Ok(products);
        }
        app.MapGet("/api/products", GetAllProducts).WithTags("Products");

        // GET BY ID
        async Task<IResult> GetProductById(int id, ProductRepository repo)
        {
            var product = await repo.GetByIdAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        }
        app.MapGet("/api/products/{id}", GetProductById).WithTags("Products");

        // POST (CREATE)
        async Task<IResult> CreateProduct(CreateProductRequest req, ProductRepository repo)
        {
            var product = await repo.CreateAsync(req);
            return Results.Ok(product);
        }
        app.MapPost("/api/products", CreateProduct).WithTags("Products");

        // PUT (UPDATE)
        async Task<IResult> UpdateProduct(int id, UpdateProductRequest req, ProductRepository repo)
        {
            var updated = await repo.UpdateAsync(id, req);
            return updated ? Results.Ok(req) : Results.NotFound();
        }
        app.MapPut("/api/products/{id}", UpdateProduct).WithTags("Products");

        // DELETE
        async Task<IResult> DeleteProduct(int id, ProductRepository repo)
        {
            var deleted = await repo.DeleteAsync(id);
            return deleted ? Results.Ok() : Results.NotFound();
        }
        app.MapDelete("/api/products/{id}", DeleteProduct).WithTags("Products");
    }
}
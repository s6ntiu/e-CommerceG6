using Cart.API.Data;
using Cart.API.DTOs;
using ECommerce.Shared.Exceptions; // IMPORTANTE: Referencia al proyecto Shared

using System.Net.Http.Json;

namespace Cart.API.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carts").WithTags("Carts");

        // Obtener carrito activo de un usuario
        group.MapGet("/{userId}", async (int userId, CartRepository repo) =>
        {
            var cart = await repo.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
            {
                throw new NotFoundException("CRT-001", "El carrito no fue encontrado para este usuario.");
            }
            return Results.Ok(cart);
        });

        // Agregar item al carrito
        group.MapPost("/items", async (AddToCartRequest request, CartRepository repo, IHttpClientFactory httpClientFactory) =>
        {
            if (request.Quantity <= 0)
                throw new BusinessRuleException("CRT-004", "Cantidad inválida.");

            var client = httpClientFactory.CreateClient("ProductsAPI");
            var response = await client.GetAsync($"/api/products/{request.ProductId}");
            
            if (!response.IsSuccessStatusCode)
                throw new NotFoundException("CRT-002", "Producto no encontrado.");

            var product = await response.Content.ReadFromJsonAsync<ProductDTO>();
            if (product == null)
                throw new NotFoundException("CRT-002", "Producto no encontrado.");

            if (product.Stock < request.Quantity)
                throw new BusinessRuleException("CRT-003", $"Stock insuficiente. Disponible: {product.Stock}, solicitado: {request.Quantity}.");

            var cart = await repo.GetActiveCartByUserIdAsync(request.UserId);

            // Si no tiene carrito activo, le creamos uno
            int cartId;
            if (cart == null)
            {
                cartId = await repo.CreateCartAsync(request.UserId);
            }
            else
            {
                if (cart.Status != "Active")
                {
                    throw new BusinessRuleException("CRT-002", "El carrito ya fue procesado o bloqueado y no se puede modificar.");
                }
                cartId = cart.Id;
            }

            await repo.AddOrUpdateItemAsync(cartId, request.ProductId, request.Quantity, product.Precio);

            return Results.Ok(new { Message = "Producto agregado exitosamente al carrito." });
        });

        // Eliminar item del carrito
        group.MapDelete("/{userId}/items/{productId}", async (int userId, int productId, CartRepository repo) =>
        {
            var cart = await repo.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
            {
                throw new NotFoundException("CRT-001", "El carrito no fue encontrado.");
            }

            if (cart.Status != "Active")
            {
                throw new BusinessRuleException("CRT-002", "El carrito ya fue procesado y no se puede modificar.");
            }

            var itemExists = cart.Items.Any(i => i.ProductId == productId);
            if (!itemExists)
            {
                throw new NotFoundException("CRT-004", "El producto no se encuentra en el carrito.");
            }

            await repo.RemoveItemAsync(cart.Id, productId);
            return Results.NoContent();
        });
    }
}


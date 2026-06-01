namespace Cart.API.DTOs;

public record AddToCartRequest(int UserId, int ProductId, int Quantity, decimal UnitPrice);

public record UpdateCartItemRequest(int Quantity);


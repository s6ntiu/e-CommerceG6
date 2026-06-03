namespace Cart.API.DTOs;

public record AddToCartRequest(int UserId, int ProductId, int Quantity);

public record UpdateCartItemRequest(int Quantity);


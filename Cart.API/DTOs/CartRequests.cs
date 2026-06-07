namespace Cart.API.DTOs;

public record AddToCartRequest(string UserId, int ProductId, int Quantity);

public record UpdateCartItemRequest(int Quantity);


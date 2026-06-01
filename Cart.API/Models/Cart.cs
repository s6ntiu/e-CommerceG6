namespace Cart.API.Models;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = "Active"; // Active, CheckedOut, Abandoned
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Propiedad de navegación
    public List<CartItem> Items { get; set; } = new();
}



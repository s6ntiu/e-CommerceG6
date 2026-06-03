namespace Order.API.DTOs
{
    public class ProductDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public long Stock { get; set; }
    }

    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
    }
}

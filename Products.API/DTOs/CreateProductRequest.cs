using System.ComponentModel.DataAnnotations;

namespace Products.API.DTOs
{
    public class CreateProductRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")] // <-- Paréntesis corregido aquí
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")] // <-- Paréntesis corregido aquí
        public int Stock { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public string Categoria { get; set; } = string.Empty;
    }
}


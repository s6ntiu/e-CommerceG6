using System.ComponentModel.DataAnnotations;
namespace Products.API.DTOs;
public class UpdateProductRequest
{
[Required(ErrorMessage = "El nombre es obligatorio.")]
[MaxLength(100)]
public string Name { get; set; } = string.Empty;
[MaxLength(500)]
public string? Description { get; set; }
[Required]
[Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
public decimal Price { get; set; }
[Required]
[Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0.")]
public int Stock { get; set; }
}
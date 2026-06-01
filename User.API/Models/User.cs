namespace User.API.Models;

public class User
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty; // Nunca se expone en los HTTP responses
    public DateTime FechaRegistration { get; set; }
    public bool Activo { get; set; }
    public int IntentosFallidos { get; set; }
}
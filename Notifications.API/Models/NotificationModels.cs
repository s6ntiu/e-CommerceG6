using System;
using System.ComponentModel.DataAnnotations;

namespace Notifications.API.Models
{
    // Esta es la entidad real que va a la base de datos
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UsuarioId { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Estado { get; set; } = "Enviada";
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }

    // Este es el DTO que recibe la petición HTTP
    public class SendNotificationRequest
    {
        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        public string Tipo { get; set; } = string.Empty;
    }
}